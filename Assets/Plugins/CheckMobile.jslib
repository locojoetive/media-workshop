mergeInto(LibraryManager.library, {
    IsMobileBrowser: function () {
        var isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
        var isTablet = (navigator.maxTouchPoints && navigator.maxTouchPoints > 2 && /MacIntel/.test(navigator.platform));
        return isMobile || isTablet;
    },
});