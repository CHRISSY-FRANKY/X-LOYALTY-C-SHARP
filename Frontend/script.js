function reviewViewportSize() { // Function to review viewport width is less than 0.66 screen width
    if (window.innerWidth < (0.5 * window.screen.width)) {
        document.querySelector('header').classList.add('viewport-less-than-screen');
        document.querySelector('form').classList.add('viewport-less-than-screen');
        document.body.style.display = 'block'; 
        document.body.style.flexDirection = 'column';
    } else {
        document.querySelector('header').classList.remove('viewport-less-than-screen');
        document.querySelector('form').classList.remove('viewport-less-than-screen');
        document.body.style.display = 'flex';
        document.body.style.flexDirection = 'row';
    }
}

reviewViewportSize(); // Run on load

window.addEventListener('resize', reviewViewportSize); // Run on resize