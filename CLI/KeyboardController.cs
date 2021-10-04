using System;
using System.Management;
using System.Threading;
using System.Drawing;

namespace LightingControl
{
    public class KeyboardController
    {
        private const int LightingWmiCmd = 131081;
        private const int LightingWmiCmdGaming = 131080;
        private const int LightingZoneCount = 4;
        private const byte Bit7 = 128;
        private const byte BrightnessLevelOn = 100;
        private const int ColorSize = 3;
        private const int ColorOffset = 25;

        private static readonly byte[] Sign = new byte[4]
        {
            (byte) 83,
            (byte) 69,
            (byte) 67,
            (byte) 85
        };

        public enum KeyboardType
        {
            Normal,
            WithNumpad,
            WithoutNumpad,
            Rgb,
        }

        private static int Execute(
            int command,
            int commandType,
            int inputDataSize,
            byte[] inputData,
            out byte[] returnData)
        {
            returnData = new byte[0];
            try
            {
                ManagementObject managementObject1 = new ManagementObject("root\\wmi",
                    "hpqBIntM.InstanceName='ACPI\\PNP0C14\\0_0'", (ObjectGetOptions) null);
                ManagementObject managementObject2 = (ManagementObject) new ManagementClass("root\\wmi:hpqBDataIn");
                ManagementBaseObject methodParameters = managementObject1.GetMethodParameters("hpqBIOSInt128");
                ManagementBaseObject managementBaseObject1 =
                    (ManagementBaseObject) new ManagementClass("root\\wmi:hpqBDataOut128");
                managementObject2["Sign"] = (object) KeyboardController.Sign;
                managementObject2["Command"] = (object) command;
                managementObject2["CommandType"] = (object) commandType;
                managementObject2["Size"] = (object) inputDataSize;
                managementObject2["hpqBData"] = (object) inputData;
                methodParameters["InData"] = (object) managementObject2;
                InvokeMethodOptions invokeMethodOptions = new InvokeMethodOptions();
                invokeMethodOptions.Timeout = TimeSpan.MaxValue;
                InvokeMethodOptions options = invokeMethodOptions;
                ManagementBaseObject managementBaseObject2 =
                    managementObject1.InvokeMethod("hpqBIOSInt128", methodParameters, options)["OutData"] as
                        ManagementBaseObject;
                returnData = managementBaseObject2["Data"] as byte[];
                return Convert.ToInt32(managementBaseObject2["rwReturnCode"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("OMEN Four zone lighting - WmiCommand.Execute occurs exception: " + ex?.ToString());
                return -1;
            }
        }

        public static bool SetZoneColors(Color[] zoneColors)
        {
            if (zoneColors.Length != 4)
                return false;
            byte[] returnData = (byte[]) null;
            int num = KeyboardController.Execute(131081, 2, 0, (byte[]) null, out returnData);
            Thread.Sleep(5);
            if (num == 0 && returnData != null)
            {
                byte[] inputData = returnData;
                returnData = (byte[]) null;
                for (int index = 0; index < 4; ++index)
                {
                    inputData[25 + index * 3] = zoneColors[index].R;
                    inputData[25 + index * 3 + 1] = zoneColors[index].G;
                    inputData[25 + index * 3 + 2] = zoneColors[index].B;
                }

                num = KeyboardController.Execute(131081, 3, inputData.Length, inputData, out returnData);
            }

            return num == 0;
        }

        public static bool IsLightingSupported()
        {
            byte[] returnData = (byte[]) null;
            byte num = 0;
            if (KeyboardController.Execute(131081, 1, 0, (byte[]) null, out returnData) == 0)
                num = (byte) ((uint) returnData[0] & 1U);
            return num == (byte) 1;
        }

        public static KeyboardType GetKeyboardType()
        {
            byte[] returnData = (byte[]) null;
            KeyboardType keyboardType = KeyboardType.Normal;
            if (KeyboardController.Execute(131080, 43, 0, (byte[]) null, out returnData) == 0)
                keyboardType = (KeyboardType) returnData[0];
            return keyboardType;
        }

        public static bool SetBrightness(byte level)
        {
            byte[] returnData = (byte[]) null;
            byte[] inputData = new byte[4]
            {
                level,
                (byte) 0,
                (byte) 0,
                (byte) 0
            };
            return KeyboardController.Execute(131081, 5, inputData.Length, inputData, out returnData) == 0;
        }

        public static bool SetFnF4Status(bool enable) => KeyboardController.SetBrightness(enable ? (byte) 228 : (byte) 100);

        public static bool IsTurnOn()
        {
            byte[] returnData = (byte[]) null;
            return KeyboardController.Execute(131081, 4, 0, (byte[]) null, out returnData) == 0 &&
                   Convert.ToBoolean((int) returnData[0] & 128);
        }
    }
}