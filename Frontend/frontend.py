import tkinter as tk
from tkinter import ttk

class ConfigApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("System Configuration")
        self.geometry("400x500")
        self.configure(padx=20, pady=20)

        self.vars = {}

        # Main frame
        main_frame = ttk.Frame(self)
        main_frame.pack(expand=True)

        # Top section for PPD and BPD checkboxes
        top_frame = ttk.Frame(main_frame)
        top_frame.pack(pady=10)

        self.add_check_group(top_frame, "PPD", ["PPD 1", "PPD 2", "PPD 3"])
        self.add_check_group(top_frame, "BPD", ["BPD 1", "BPD 2", "BPD 3"])

        # Radiobuttons in pairs
        pair_frame = ttk.Frame(main_frame)
        pair_frame.pack()

        radio_groups = [
            ("VCU", ["VCU 1", "VCU 2"]),
            ("Debug", ["Debug", "Not Debug"]),
            ("Voltage", ["400V", "800V"]),
            ("Driver Mode", ["LHD", "RHD"]),
            ("Traction", ["AWD", "RWD"]),
            ("Level", ["LVL 4-1", "LVL 2-1"])
        ]

        for i in range(0, len(radio_groups), 2):
            row_frame = ttk.Frame(pair_frame)
            row_frame.pack(pady=10)
            for j in range(2):
                if i + j < len(radio_groups):
                    group_name, options = radio_groups[i + j]
                    self.add_radio_group(row_frame, group_name, options)

        # Submit button
        submit_btn = ttk.Button(self, text="Download Baseline", command=self.print_config)
        submit_btn.pack(pady=20)

    def add_check_group(self, parent, group_name, options):
        frame = ttk.LabelFrame(parent, text=group_name)
        frame.pack(side='left', padx=10)
        self.vars[group_name] = {}
        for option in options:
            var = tk.BooleanVar()
            chk = ttk.Checkbutton(frame, text=option, variable=var)
            chk.pack(anchor='w')
            self.vars[group_name][option] = var

    def add_radio_group(self, parent, group_name, options):
        frame = ttk.LabelFrame(parent, text=group_name)
        frame.pack(side='left', padx=20)
        var = tk.StringVar()
        var.set(options[0])  # Default value
        self.vars[group_name] = var
        for option in options:
            rbtn = ttk.Radiobutton(frame, text=option, variable=var, value=option)
            rbtn.pack(anchor='w')

    def print_config(self):
        config_data = {}
        for group, value in self.vars.items():
            if isinstance(value, dict):  # Checkboxes
                selected = [k for k, v in value.items() if v.get()]
                config_data[group] = selected
            else:  # Radiobuttons
                config_data[group] = value.get()

        # Build output string
        output = []
        for k, v in config_data.items():
            if isinstance(v, list):
                output.append(f"{k}: {', '.join(v) if v else 'None'}")
            else:
                output.append(f"{k}: {v}")

        final_string = " | ".join(output)
        print("Selected configuration:")
        print(final_string)

        # This final_string can now be passed to a backend function
        # e.g., send_to_backend(final_string)

if __name__ == "__main__":
    app = ConfigApp()
    app.mainloop()
