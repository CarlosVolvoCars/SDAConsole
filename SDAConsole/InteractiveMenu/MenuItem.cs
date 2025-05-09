namespace InteractiveApp.InteractiveMenu
{
    public class MenuItem
    {
        public string Title { get; }
        public List<MenuItem> Children { get; }
        public Func<Task>? Action { get; }


        public MenuItem(string title, List<MenuItem> children)
        {
            Title = title;
            Children = children;
        }

        public MenuItem(string title, Func<Task> action)
        {
            Title = title;
            Action = action;
            Children = new List<MenuItem>();
        }
    }
}
