namespace InteractiveApp.Services
{
    public static class HelpService
    {
        public static void ControlsGuide()
        {
            Console.WriteLine("Controls Guide:");
            Console.WriteLine("1. Use the arrow keys to navigate through the menu.");
            Console.WriteLine("2. Press Up to move up and Down to move down.");
            Console.WriteLine("3. Press Right to select an option.");
            Console.WriteLine("4. Press Left to go back to the previous menu.");
            Console.WriteLine("5. Press Esc to exit the application.");
            Console.WriteLine("6. Press Backspace to go back to the previous menu.");
            Console.WriteLine("7. Use Enter to confirm selections (Non-menu items selection). Or when prompted to enter a value. Or when prompted to Press any key to continue.");
            Console.WriteLine("4. Use Ctrl+C to exit the application at any time.");
            Console.WriteLine("5. Follow on-screen instructions for specific tasks.");
            Console.WriteLine("6. For more help, refer to the documentation or contact support.");
            Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
        }
    }
}