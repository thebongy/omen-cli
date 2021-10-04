using System.Drawing;

namespace LightingControl
{
    public class CommandHandlers
    {
        public static int SetFourCommand(Color col1, Color col2, Color col3, Color col4)
        {
            KeyboardController.SetZoneColors(
                new Color[4]
                {
                    col1,
                    col2,
                    col3,
                    col4,
                }
                );
            return 0;
        }

        public static int SetOneCommand(Color col)
        {
            KeyboardController.SetZoneColors(
                new Color[4]
                {
                    col,
                    col,
                    col,
                    col
                }
            );
            return 0;
        }
    }
}