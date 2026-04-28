window.aprilcraftTheme = window.aprilcraftTheme || {
    setTheme: function (theme) {
        var root = document.documentElement;
        if (!root) {
            return;
        }

        root.setAttribute("data-theme", theme === "light" ? "light" : "dark");
    }
};
