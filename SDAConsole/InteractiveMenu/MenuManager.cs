using InteractiveApp.Config;
using SDAConsole.types;
using Microsoft.Extensions.Logging;

namespace InteractiveApp.InteractiveMenu
{
    public class MenuManager
    {
        private MenuContext context;

        private List<ILogger> availableLoggers;

        public MenuManager(List<ILogger> availableLoggers)
        {
            this.availableLoggers = availableLoggers;

            context = new MenuContext(this.availableLoggers);
        }

        public async Task Run()
        {
            while (true)
            {
                Console.Clear();
                context.DisplayCurrentMenu();

                var key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        context.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        context.MoveDown();
                        break;
                    case ConsoleKey.RightArrow:
                        await context.Select();
                        break;
                    case ConsoleKey.LeftArrow:
                        context.GoBack();
                        break;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Backspace:
                        context.GoBack();
                        break;
                }
            }
        }
    }
}
