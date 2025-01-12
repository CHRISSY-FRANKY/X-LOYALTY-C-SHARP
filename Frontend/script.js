function reviewViewportSize() { // Function to review viewport width is less than 0.66 screen width
    if (window.innerWidth < (0.66 * window.screen.width)) {
        document.querySelector('header').classList.add('viewport-less-than-screen');
    } else {
        document.querySelector('header').classList.remove('viewport-less-than-screen');
    }
}

reviewViewportSize(); // Run on load

window.addEventListener('resize', reviewViewportSize); // Run on resize