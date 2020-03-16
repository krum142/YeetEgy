function resizeCategory() {
    if (Math.max(document.documentElement.clientWidth, window.innerWidth || 0) <= 992) {
        document.getElementById("catPos").classList.remove('position-fixed');
        document.getElementById("catWid").style.width = "330px";
    } if (Math.max(document.documentElement.clientWidth, window.innerWidth || 0) >= 991) {
        document.getElementById("catWid").style.width = "220px";
        document.getElementById("catPos").classList.add('position-fixed');
    }
}