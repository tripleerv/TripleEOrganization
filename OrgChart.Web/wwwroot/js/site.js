function DisplayMessage(type, message) {
    var nav_bar = document.getElementById('nav-bar');

    const div = document.createElement('div');
    div.classList.add('alert', 'alert-timed', 'alert-dismissible', 'fade', 'show', 'shadow-lg', 'position-fixed');
    div.setAttribute('role', 'alert');
    $(div).css('z-index', '1050');
    $(div).css('top', '1rem');
    $(div).css('right', '1rem');

    nav_bar.appendChild(div);

    switch (type) {
        case 'Success': div.classList.add('alert-success'); div.innerHTML = '<strong>Success! </strong>' + message + '<i class="fa-solid fa-circle-check ms-2"></i>'; break;
        case 'Fail': div.classList.add('alert-danger'); div.innerHTML = '<strong>Fail! </strong>' + message + '<i class="fa-solid fa-circle-xmark ms-2"></i>'; break;
        case 'Scan': div.classList.add('alert-success'); div.innerHTML = '<strong>Success! </strong><i class="fa-solid fa-circle-check ms-2"></i>'; break;
        default: div.classList.add('alert-info'); div.innerHTML = '<strong>Info! </strong>' + message + '<i class="fa-solid fa-circle-info ms-2"></i>'; break;
    }

    const slideFade = (ele) => {
        const fade = { opacity: 0, transition: 'opacity 1200ms' };
        ele.css(fade).slideUp(700);
    };

    setTimeout(function () {
        slideFade($(div));
    }, 2000);
    setTimeout(function () {
        $(div).remove();
    }, 2500);
}