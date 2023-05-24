
let ul_employees = document.getElementById('ul-employees');

let modal_top_employees = document.getElementById('modal-ranked-user-list');
let modal_body = document.getElementById('modal-ranked-user-list-body');

let cur_input_name;
let cur_location_id = '';
let employees = [];
let level_units = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve" ];

modal_top_employees.addEventListener('show.bs.modal', (event) => { GetTopEmployeeMatches($(event.relatedTarget).data('location-id')); });

$(document).ready((e) => { GetEmployees(); });
$(document).on('focus', '.input-employee-name', (event) => {
    var selected = event.target;
    cur_input_name = selected;
    cur_location_id = $(selected).data('location-id');

    var selectedTop = $(selected).position().top;
    var selectedLeft = $(selected).position().left;

    var targetTop = selectedTop + $(selected).outerHeight();
    var targetLeft = selectedLeft - 1;

    $(ul_employees).css(
        {
            top: targetTop,
            left: targetLeft,
            display: 'block'
        });
});
$(document).on('click', '.btn-collapse-location-level', (event) => {

    var target = event.currentTarget;
    var level = $(target).data('level');

    $('.' + level).toggleClass('show hide');


});
$(document).on('click', '.employee-search-results-item', (event) => {

    var target = event.currentTarget;

    var employee_id = $(target).data('employee-id');
    var first_name = $(target).data('employee-first-name');
    var last_name = $(target).data('employee-last-name');
    var full_name = first_name + ' ' + last_name;

    if (cur_input_name != undefined) {
        if (employee_id == '0') {
            $(cur_input_name).val('');
        }
        else {
            $(cur_input_name).val(full_name);
        }

        UpdateEmployee(cur_location_id, employee_id);
        BuildEmployeeList();
    }

});
$(document).on('focusout','.input-employee-name', (event) => {

    setTimeout((event) => {
        cur_input_name = undefined;
        cur_location_id = '';
        $(ul_employees).css(
            {
                display: 'none'
            });
    }, 100);

});
$(document).on('keyup', '.input-employee-name', (event) => {
    var target = event.currentTarget;
    var value = $(target).val();

    BuildEmployeeList(value);
});
$(document).on('change', '.input-location-name', (event) => {
    var target = event.currentTarget;
    var location_id = $(target).data('location-id');
    var location_name = $(target).val();

    UpdateLocation(location_id, location_name);
});
$(document).on('click', '.btn-add-location', (event) => {
    var target = event.currentTarget;
    var location_id = $(target).data('location-id');
    var parent_location_id = $(target).data('parent-location-id');
    var level = $(target).data('level');

    AddLocation(parent_location_id, location_id, level);
});
$(document).on('click', '.btn-remove-location', (event) => {
    var target = event.currentTarget;
    var location_id = $(target).data('location-id');

    RemoveLocation(location_id);
});


//#region HOVER Functions

$(function () {
    $('.div-group').hover(function () {
        var add = $(this).children().find('.btn-add-location');
        var remove = $(this).children().find('.btn-remove-location');
        var top = $(this).find('.btn-top-matches');
        var div = $(this).find('.div-buttons');

        $(div).css('display', 'inline-flex');
        $(add).css('display', 'flex');
        $(remove).css('display', 'flex');
        $(top).css('display', 'flex');
    }, function () {
        var add = $(this).children().find('.btn-add-location');
        var remove = $(this).children().find('.btn-remove-location');
        var top = $(this).find('.btn-top-matches');
        var div = $(this).find('.div-buttons');

        $(div).css('display', 'none');
        $(add).css('display', 'none');
        $(remove).css('display', 'none');
        $(top).css('display', 'none');
    });

    $('.btn-add-location').hover(function () {
        var location_id = $(this).data('location-id');
        var counter = $(this).data('counter');

        $('#div-' + counter).css('border-bottom', '2px solid lightgreen');
        $('#div-children-' + location_id).css('border-bottom', '2px solid lightgreen');
    }, function () {
        var location_id = $(this).data('location-id');
        var counter = $(this).data('counter');

        $('#div-' + counter).css('border-bottom', 'none');
        $('#div-children-' + location_id).css('border-bottom', 'none');
    });

});

//#endregion

//#region Functions

function BuildEmployeeList(text) {
    $(ul_employees).empty();
    $(ul_employees).append(`
            <li tabindex="1" class="dropdown-item employee-search-results-item" data-employee-id="0">
                    <div class="employee-search-results-row">
                        <span class="text-muted"> -Unassign -</span>
                    </div>
            </li>`);

    if (text == undefined || text == '') {
        for (var i = 0; i < employees.length; i++) {
            var emp = employees[i];
            $(ul_employees).append(`
            <li class="dropdown-item employee-search-results-item" data-employee-id="${emp.employeeId}" data-employee-first-name="${emp.firstName}" data-employee-last-name="${emp.lastName}" >
                    <div class="employee-search-results-row">
                        <span>${emp.firstName}</span>
                        <span>${emp.lastName} </span>
                            <span> (${emp.employeeId})</span>
                    </div>
            </li>`);
        }
    }
    else {
        text = text.toUpperCase();
        for (var j = 0; j < employees.length; j++) {
            var emp = employees[j];

            var e_fullName = emp.fullName.toUpperCase();
            var e_employeeId = emp.employeeId.toUpperCase();

            var li = `<li class="dropdown-item employee-search-results-item" data-employee-id="${emp.employeeId}" data-employee-first-name="${emp.firstName}" data-employee-last-name="${emp.lastName}" >
                        <div class="employee-search-results-row">
                            <span>${emp.firstName}</span>
                            <span>${emp.lastName} </span>
                            <span> (${emp.employeeId})</span>
                        </div>
                      </li>`;

            if (e_fullName.includes(text)) {
                $(ul_employees).append(li);
            }
            else if (e_employeeId.includes(text)) {
                $(ul_employees).append(li);
            }
        }
    }
}
function CreateElements(parent_location_id, location_id, level) {

    const parent_div = document.getElementById('collapse-' + parent_location_id);

    const div_collapse = document.createElement('div');
    div_collapse.setAttribute('id', 'collapse-' + location_id);
    div_collapse.setAttribute('tabindex', '-1');
    div_collapse.classList.add('collapse', 'show', level_units[level+1], 'collapse-' + parent_location_id,);

    const div_flex = document.createElement('div');
    div_flex.classList.add('d-flex', 'div-hover');

    const input_location = document.createElement('input');
    input_location.setAttribute('id', 'input-location-' + location_id);
    input_location.classList.add('form-control', 'form-control-sm', 'input-location-name');
    input_location.setAttribute('type', 'text');
    input_location.setAttribute('value', '');
    input_location.setAttribute('data-location-id', location_id);

    const input_employee = document.createElement('input');
    input_employee.setAttribute('id', 'input-employee-' + location_id);
    input_employee.classList.add('form-control', 'form-control-sm', 'input-employee-name');
    input_employee.setAttribute('type', 'text');
    input_employee.setAttribute('tabindex', '-1');
    input_employee.setAttribute('value', '');
    input_employee.setAttribute('data-location-id', location_id);

    const div_hover = document.createElement('div');
    div_hover.classList.add('div-hover-buttons');

    const button = document.createElement('button');
    button.classList.add('btn', 'btn-sm', 'btn-success', 'btn-add-location');
    button.setAttribute('type', 'button');
    button.setAttribute('tabindex', '-1');
    button.setAttribute('data-location-id', location_id);
    button.setAttribute('data-parent-location-id', parent_location_id);
    button.setAttribute('data-level', level+1);
    button.innerHTML = 'ADD';



    div_flex.appendChild(input_location);
    div_flex.appendChild(input_employee);

    div_hover.appendChild(button);

    div_flex.appendChild(div_hover);

    div_collapse.appendChild(div_flex);
    parent_div.appendChild(div_collapse)
}

//#endregion

//#region AJAX / JSON

function UpdateLocation(location_id, location_name) {

    $.getJSON(`?handler=UpdateLocationName&LocationId=${location_id}&LocationName=${location_name}`, (data) => {
        if (data != null) {
            DisplayMessage(data.status, data.message);
        }
        else {
            DisplayMessage('Fail', 'Location Name Not Updated.');
        }
    });
}
function AddLocation(parent_location_id, location_id, level) {

    $.getJSON(`?handler=AddLocation&ParentLocationId=${location_id}`, (data) => {
        if (data != null) {
            if (data.status == 'Success') {
                CreateElements(location_id, data.locationId, level);
            }
            DisplayMessage(data.status, data.message);
        }
        else {
            DisplayMessage('Fail', 'Refresh Page!');
        }
    });
}
function RemoveLocation(location_id) {    

    $.getJSON(`?handler=RemoveLocation&LocationId=${location_id}`, (data) => {
        if (data != null) {
            DisplayMessage(data.status, data.message);

            $('#collapse-' + location_id).remove();
        }
        else {
            DisplayMessage('Fail', 'Location Not Removed.');
        }
    });
}
function UpdateEmployee(location_id, employee_id) {
    $.getJSON(`?handler=UpdateEmployee&LocationId=${location_id}&EmployeeId=${employee_id}`, (data) => {
        if (data != null) {
            DisplayMessage(data.status, data.message);
        }
        else {
            DisplayMessage('Fail', 'Location Name Not Updated.');
        }
    });
}
function GetEmployees() {
    $.getJSON(`?handler=Users`, (data) => {
        $.each(data, function (i, e) {
            if (e.status == 'A') {
                employees.push(e);
            }
        });
        BuildEmployeeList();
    });
}
function GetTopEmployeeMatches(location_id) {

    $('#input-export-location-id').val(location_id);
    var url = `?handler=TopEmployeeMatchesPartial&LocationId=${location_id}`;

    $(modal_body).load(url, (data) => {
    });
}

//#endregion
