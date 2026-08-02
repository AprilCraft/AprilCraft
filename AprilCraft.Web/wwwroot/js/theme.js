// AprilCraft Client Utilities
(function () {
    window.aprilcraftTheme = {
        getTheme: function () {
            try {
                return localStorage.getItem("aprilcraft-theme") || "dark";
            } catch (e) {
                return "dark";
            }
        },
        setTheme: function (theme) {
            var mode = theme === "light" ? "light" : "dark";
            var root = document.documentElement;
            if (root) {
                root.setAttribute("data-theme", mode);
            }
            try {
                localStorage.setItem("aprilcraft-theme", mode);
            } catch (e) {}
            return mode;
        },
        initTheme: function () {
            var theme = this.getTheme();
            this.setTheme(theme);
            return theme;
        },
        copyToClipboard: async function (text) {
            try {
                if (navigator.clipboard && window.isSecureContext) {
                    await navigator.clipboard.writeText(text);
                    return true;
                } else {
                    var textArea = document.createElement("textarea");
                    textArea.value = text;
                    textArea.style.position = "fixed";
                    textArea.style.left = "-999999px";
                    textArea.style.top = "-999999px";
                    document.body.appendChild(textArea);
                    textArea.focus();
                    textArea.select();
                    var successful = document.execCommand("copy");
                    textArea.remove();
                    return successful;
                }
            } catch (err) {
                console.error("Failed to copy text: ", err);
                return false;
            }
        },
        downloadFile: function (url, filename) {
            try {
                var a = document.createElement("a");
                a.href = url;
                a.download = filename || "aprilcraft-design";
                document.body.appendChild(a);
                a.click();
                a.remove();
                return true;
            } catch (e) {
                console.error("Download failed:", e);
                return false;
            }
        }
    };

    // Initialize theme immediately on script load to prevent flash of wrong theme
    window.aprilcraftTheme.initTheme();

    // Pure JS scroll listener for instantaneous navbar backdrop effect without Blazor signal latency
    function initNavbarScroll() {
        var nav = document.querySelector(".ac-nav");
        if (!nav) return;
        
        function onScroll() {
            if (window.scrollY > 20) {
                nav.classList.add("ac-nav--scrolled");
            } else {
                nav.classList.remove("ac-nav--scrolled");
            }
        }
        
        window.removeEventListener("scroll", onScroll);
        window.addEventListener("scroll", onScroll, { passive: true });
        onScroll();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initNavbarScroll);
    } else {
        initNavbarScroll();
    }

    // Re-bind when navigation occurs
    window.addEventListener("popstate", initNavbarScroll);
})();
