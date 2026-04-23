document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
});

function toggleDarkMode() {
    body.classList.toggle("dark-mode");
    localStorage.setItem("theme", body.classList.contains("dark-mode") ? "dark" : "light");
}
if (localStorage.getItem("theme") == "dark") {
    body.classList.add("dark-mode")
}
window.toggleDarkMode = toggleDarkMode

function toggleHighContrast() {
    body.classList.toggle("high-contrast");
    localStorage.setItem("theme", body.classList.contains("high-contrast") ? "high-contrast" : "normal");
}
if (localStorage.getItem("contrastMode") == "high-contrast") {
    body.classList.add("high-contrast")
}
window.toggleHighContrast = toggleHighContrast

function speakText() {
    let text = document.body.innerText;
    let speech = new SpeechSynthesisUtterance(text);
    speech.lang = 'en-GB';
    window.speechSynthesis.speak(speech);
}
window.speakText = speakText

function changeFontSize(action) {
    let currentSize = parseInt(window.getComputedStyle(body).fontSize);
    if (action === 'increase') {
        currentSize += 2;
    } else if (action === 'decrease') {
        currentSize -= 2;
    }
    body.style.fontSize = currentSize + "px";
    localStorage.setItem("fontSize",currentSize = "px")
}
window.changeFontSize = changeFontSize