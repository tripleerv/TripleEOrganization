// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var copied_employee_id = 0;
var copied_ids = [];
var s_employee = "";
var timeout;
var current_collapse_level = ""

//#region Main Event Handlers

window.onload = function () {
    var collapse_state = $('#input-collapse-state').val();

    if (collapse_state != null) {

        current_collapse_level = collapse_state;

        switch (collapse_state) {
            case "level-1": CollapseLevel(1); break;
            case "level-2": CollapseLevel(2); break;
            case "level-3": CollapseLevel(3); break;
            case "level-4": CollapseLevel(4); break;
            case "level-5": CollapseLevel(5); break;
            case "level-6": CollapseLevel(6); break;
        }
    }
}

$(function () {    
    //Add
    $(document).on('click', '.btn-add' , function (e) {
        var id = $(this).val();
        Add(id);        
    });
    //Add Save
    $(document).on('click', '.btn-save-add', function (e) {
        var id = $(this).val();
        SaveAdd(id);
    });
    //Edit
    $(document).on('click', '.btn-edit', function (e) {
        var id = $(this).val();
        var type = $(this).data('type');
        var employee_id = $(this).data('employee-id');
        var description = $(this).data('description');

        Edit(id, type, employee_id, description);        
    });
    //Edit Save
    $(document).on('click', '.btn-save-edit', function (e) {
        var id = $(this).val();
        SaveEdit(id);
    });
    //Cancel
    $(document).on('click', '.btn-cancel', function (e) {
        $('.edit-inputs').hide();
        $('.edit-inputs').empty();
        $('.list-details').show();
        $('#li-new-item').remove();
    });
    //Remove
    $(document).on('click', '.btn-remove', function (e) {
        var id = $(this).val();
        Remove(id);
    });       
    //Select Type
    $(document).on('change', '.select-type', function (e) {
        var value = $(this).val();
        if (value == 1) { //Personnel
            GetEmployees(0);
        }
        else {
            $('#select-employee').prop('disabled', true);
        }
    });
    //Checkbox
    $(document).on('click', 'input[type="checkbox"]', function (e) {
        var id = $(this).data('id');
        var parent_id = $(this).data('parent-id');

        CheckMe(id, parent_id); 
    });
    //Paste
    $(document).on('click', '.btn-paste', function (e) {
        var parent_id = $(this).val();

        Paste(parent_id);
    }); 
    //Move UP
    $(document).on('click', '.btn-move-up', function (e) {
        var id = $(this).data('hierarchy-id');

        MoveUp(id);
    }); 
    //Move DOWN
    $(document).on('click', '.btn-move-down', function (e) {
        var id = $(this).data('hierarchy-id');

        MoveDown(id);
    }); 
    // COPY Employee From Table
    $(document).on('click', '.employee-table-copy', function (e) {
        var employee_id = $(this).data('employee-id');
        var text = $(this).text();

        CopyFromTable(employee_id, text);
    }); 
});
//#endregion

//#region Main Functions
function Paste(parent_id) {

    if (copied_ids.length === 0) {

        $.ajax({
            type: 'post',
            url: '?handler=Add',
            data: { AddType: 1, AddDescription: 'NO ROLE', AddEmployeeId: copied_employee_id, AddParentId: parent_id, CollapseState: current_collapse_level },
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (results) {
            ShowMessageBanner(results.status, results.message);
            if (results.status == "Success") {
                window.location = "";
                //SetAddNew(id, results.id, results.type, results.description, results.employeeId, results.employeeFullName);
            }
        });
    }
    else {
        var json_data = { ChangeParentId: parent_id, ChangeIds: copied_ids, CollapseState: current_collapse_level };

        $.ajax({
            type: 'post',
            url: '?handler=ChangeParent',
            data: json_data,
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (results) {
            window.location = "";
        });
    }    
}

function Edit(id, type, employee_id, description) {
    $('.edit-inputs').hide();
    $('.edit-inputs').empty();
    $('.list-details').show();
    $('#li-new-item').remove();

    $('#li-details-' + id).hide();

    var edit_div = $('#edit-inputs-' + id);

    edit_div.css('display', 'flex').show();
    edit_div.append(`            
                <div class="col-2 p-0 pr-1">
                    <select id="select-type" class="form-control form-control-sm select-type">
                        <option disabled hidden selected>Select Type</option>
                        <option value="0">Department</option>
                        <option value="1">Personnel</option>
                    </select>
                </div>
                <div class="col-4 p-0 pr-1">
                    <input id="input-description" class="form-control form-control-sm" type="text" placeholder="Please Enter Description" />
                </div>
                <div class="col-3 p-0 pr-1">
                    <select id="select-employee" class="form-control form-control-sm select-employee" disabled>
                        <option disabled hidden selected>Select Employee</option>
                    </select>
                </div>
                <div class="col p-0"><button type="button" class="btn btn-sm btn-save-edit" value="${id}">Save</button></div>
                <div class="col p-0"><button type="button" class="btn btn-sm btn-cancel pt-1 float-right">Cancel</button></div>           
            `);
    $('#select-type').val(type);
    $('#input-description').val(description);

    if (type == 1) {
        $('#select-employee').attr('disabled', false);
        GetEmployees(employee_id);
    }    
}

function Add(id) {
    $('.edit-inputs').hide();
    $('.edit-inputs').empty();
    $('.list-details').show();
    $('#li-new-item').remove();

    var parent = $('#ul-' + id);

    $(parent).append(`
            <li id="li-new-item" class="py-1">
                <div class="col-2 p-0 pr-1">
                    <select id="select-type" class="form-control form-control-sm select-type">
                        <option disabled hidden selected>Select Type</option>
                        <option value="0">Department</option>
                        <option value="1">Personnel</option>
                    </select>
                </div>
                <div class="col-4 p-0 pr-1">
                    <input id="input-description" class="form-control form-control-sm" type="text" placeholder="Please Enter Description" />
                </div>
                <div class="col-3 p-0 pr-1">
                    <select id="select-employee" class="form-control form-control-sm select-employee" disabled>
                        <option disabled hidden selected>Select Employee</option>
                    </select>
                </div>
                <div class="col p-0"><button type="button" class="btn btn-sm btn-save-add" value="${id}">Save</button></div>
                <div class="col p-0"><button type="button" class="btn btn-sm btn-cancel pt-1 float-right">Cancel</button></div>
            </li>
            `);
}

function Remove(id) {
    var item = $('#li-' + id);
    var list = $('#ul-' + id);

    $.ajax({
        type: 'post',
        url: '?handler=Remove',
        data: { RemoveId: id },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (results) {
        ShowMessageBanner(results.status, results.message);
        if (results.status == "Success") {
            $(item).remove();
            $(list).remove();

            if (results.employeeId != 0) {
                $('#td-' + results.employeeId).removeClass('exists');
            }            
        }
    });
}

function SaveAdd(id) {
    window.event.preventDefault();
    
    var type = $('#select-type').val();
    var description = $('#input-description').val();
    var employee_id = $('#select-employee').val();

    if (type != null && type != undefined && description != "") {
        $.ajax({
            type: 'post',
            url: '?handler=Add',
            data: { AddType: type, AddDescription: description, AddEmployeeId: employee_id, AddParentId: id },
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (results) {
            ShowMessageBanner(results.status, results.message);
            if (results.status == "Success") {
                SetAddNew(id, results.id, results.type, results.description, results.employeeId, results.employeeFullName);
            }
        });
    }
    else {
        ShowMessageBanner("Fail", "Type or Description cannot be blank.");
        if (type == null && type == undefined) {
            $('#select-type').css('border', '1px solid #dc3545');
        }
        if (description == '') {
            $('#input-description').css('border', '1px solid #dc3545');
        }
    }    
}

function SaveEdit(id) {
    window.event.preventDefault();

    var type = $('#select-type').val();
    var description = $('#input-description').val();
    var employee_id = $('#select-employee').val();

    if (type != null && type != undefined && description != "") {
        $.ajax({
            type: 'post',
            url: '?handler=Edit',
            data: { EditId: id, EditType: type, EditDescription: description, EditEmployeeId: employee_id },
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (results) {
            ShowMessageBanner(results.status, results.message);
            SetEdit(id, results.type, results.description, results.employeeId, results.employeeFullName);
        });
    }
    else {
        ShowMessageBanner("Fail", "Type or Description cannot be blank.");
        if (type == null && type == undefined) {
            $('#select-type').css('border', '1px solid #dc3545');
        }
        if (description == '') {
            $('#input-description').css('border', '1px solid #dc3545');
        }
    }    
}

function ShowMessageBanner(type, message) {
    var main_body = document.getElementById('row-message');

    const div2 = document.createElement('div');
    div2.classList.add('alert', 'alert-hover', 'alert-timed', 'alert-dismissible', 'fade', 'show', 'align-middle', 'shadow-lg');
    div2.setAttribute('role', 'alert');

    const button = document.createElement('button');
    button.classList.add('close');
    button.setAttribute('type', 'button');
    button.setAttribute('data-dismiss', 'alert');
    button.setAttribute('aria-label', 'close');
    button.innerHTML = '<span aria-hidden="true">&times;</span>';

    main_body.appendChild(div2);
    div2.appendChild(button);

    switch (type) {
        case 'Success': div2.classList.add('alert-success'); div2.innerHTML = '<strong>Success! </strong>' + message; break;
        case 'Fail': div2.classList.add('alert-danger'); div2.innerHTML = '<strong>Fail! </strong>' + message; break;
        case 'Info': div2.classList.add('alert-info'); div2.innerHTML = '<strong>Info! </strong>' + message; break;
    }

    $(div2).delay(2000).fadeTo(500, 0).slideUp(500, function () {
        $(div2).remove();
    });
}

function SetAddNew(parent_id, id, type, description, employee_id, employee_name) {
    $('#li-new-item').remove();
    var children = $('#ul-' + id).find('input[type="checkbox"]');

    var html_collapse = '';
    var html_remove = '';
    var html_employee = '';
    var html_description = '';

    if (children.length > 0) {
        
        //html_remove = '<button type="button" class="btn btn-sm btn-remove" disabled><i class="fas fa-minus"></i></button>';
    }
    else {
        html_remove = `<button type="button" class="btn btn-sm btn-remove" value="${id}"><i class="fas fa-minus"></i></button>`;
    }

    if (type == 1) {
        html_description = `<span id="description-${id}" class="personnel-description">${description}</span>`;
        html_employee = `<span id="name-${id}" class="personnel-name">${employee_name}</span>`;
    }
    else {
        html_description = `<span id="description-${id}" class="department-description">${description}</span>`;
        html_employee = `<span id="name-${id}" class="department-name"></span>`;
    }

    $('#ul-' + parent_id).append(`
        <li id="li-${id}" data-id="${id}">
            <div id="li-details-${id}" class="list-details">
                <div>
                    <div id="move-list-${id}" class="move-list">
                        <button id="btn-move-up-${id}" class="btn btn-sm btn-move-up" type="button" data-hierarchy-id="${id}" disabled><i class="fas fa-angle-double-up"></i></button>
                        <button id="btn-move-down-${id}" class="btn btn-sm btn-move-down" type="button" data-hierarchy-id="${id}" disabled><i class="fas fa-angle-double-down"></i></button>
                        <button id="btn-collapse-level-" class="btn btn-sm btn-collapse-level show" type="button" data-level="" title="Show Level"><i class="fas fa-expand"></i></button>
                    </div>
                    ${html_description}
                </div>
                 <div>    
                    ${html_employee}
                    <button id="btn-paste-${id}" type="button" class="btn btn-sm btn-paste" value="${id}">Paste</button>
                    <div id="button-list-${id}" class="button-list">
                        <input id="checkbox-${id}" type="checkbox" class="form-check-input" data-id="${id}" data-parent-id="${parent_id}" />
                        <button id="btn-add-${id}" type="button" class="btn btn-sm btn-add" value="${id}"><i class="fas fa-plus"></i></button>
                        <button id="btn-edit-${id}" type="button" class="btn btn-sm btn-edit" value="${id}" data-type="${type}" data-employee-id="${employee_id}" data-description="${description}"><i class="fas fa-pen"></i></button>
                        ${html_remove}
                    </div>
                </div>
            </div>
            <div id="edit-inputs-${id}" class="edit-inputs"></div>
        </li>
    <ul id="ul-${id}"></ul>`
    );    

    $('#td-' + employee_id).attr('data-hierarchy-id', id);
    $('#td-' + employee_id).data('hierarchy-id', id);
    $('#td-' + employee_id).addClass('exists');
}    

function SetEdit(id, type, description, employee_id, employee_name) {
    $('.edit-inputs').hide();
    $('.edit-inputs').empty();
    $('.list-details').show();
    $('#li-new-item').remove();
    $('#description-' + id).removeClass('personnel-description');
    $('#description-' + id).removeClass('department-description');

    if (type == 1) {
        $('#description-' + id).addClass('personnel-description');
        $('#description-' + id).text(description);
        $('#name-' + id).text(employee_name);
        $('#td-' + employee_id).addClass('exists');
    }
    else {
        $('#description-' + id).addClass('department-description');
        $('#description-' + id).text(description);
        $('#name-' + id).text('');
    }

    //Set Edit Button Description & Employee_Id
    $('#btn-edit-' + id).attr('data-description', description);    
    $('#btn-edit-' + id).data('description', description);    

    $('#btn-edit-' + id).attr('data-employee-id', employee_id);    
    $('#btn-edit-' + id).data('employee-id', employee_id);  

    $('#btn-edit-' + id).attr('data-type', type);
    $('#btn-edit-' + id).data('type', type);

    $('#td-' + employee_id).attr('data-hierarchy-id', id);
    $('#td-' + employee_id).data('hierarchy-id', id);
}  

function CheckMe(id, parent_id) {

    var is_checked = $('#checkbox-' + id).prop('checked');
    var children = $('#ul-' + id).find('input[type="checkbox"]');

    if (is_checked == true) {
        copied_ids.push(id);

        $(children).prop('checked', true);
        $(children).prop('disabled', true);  
        
        $('#ul-' + id).css('color', '#009cde');
        $('#li-' + id).css('color', '#009cde');
    }
    else {
        var index = copied_ids.indexOf(id);
        if (index !== -1) {
            copied_ids.splice(index, 1);
        }

        $(children).prop('checked', false);
        $(children).prop('disabled', false);

        $('#li-' + id).css('color', '#212529');
        $('#ul-' + id).css('color', '#212529');
    }

    // Paste Buttons
    $('.btn-paste').css('visibility', 'hidden');
    var all_li = $.find('li');
    var all_cb = $(all_li).find('input[type="checkbox"]');
    var any_checked = $('input:checkbox:checked').length > 0;

    if (any_checked) {
        $(all_cb).each(function () {
            var id = $(this).data('id');

            if (!$(this).prop('checked')) {
                if (id != parent_id) {
                    $('#btn-paste-' + id).css('visibility', 'visible');
                }
            }
        });
    }
}

function MoveUp(id) {
    $.ajax({
        type: 'post',
        url: '?handler=MoveUp',
        data: { EditId: id, CollapseState: current_collapse_level },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (results) {
        ShowMessageBanner(results.status, results.message);
        //if (results.status == "Success") {
        //    SetAddNew(id, results.id, results.type, results.description, results.employeeId, results.employeeFullName);
        //}
        window.location = "";
    });
}

function MoveDown(id) {
    $.ajax({
        type: 'post',
        url: '?handler=MoveDown',
        data: { EditId: id, CollapseState: current_collapse_level },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (results) {
        ShowMessageBanner(results.status, results.message);
        //if (results.status == "Success") {
        //    SetAddNew(id, results.id, results.type, results.description, results.employeeId, results.employeeFullName);
        //}
        window.location = "";
    });
}

function CopyFromTable(employee_id, text) {
    $('.employee-table-copy').removeClass('show');
    $('td').removeClass('copied');

    if (text == 'Copy') {
        $(this).text('Cancel');
        $(this).addClass('show');
        $('#td-' + employee_id).addClass('copied');
        $('.btn-paste').css('visibility', 'visible');
        copied_employee_id = employee_id;
    }
    else {
        $(this).text('Copy');
        $('.btn-paste').css('visibility', 'hidden');
        copied_employee_id = 0;
    }
}

function GetEmployees(employee_id) {
    $('#select-employee').empty();
    $('#select-employee').append(`<option selected hidden disabled>Select Employee</option>`);
    $('#select-employee').append(`<option value=""></option>`);
    $('#select-employee').attr('disabled', false);

    $.getJSON(`?handler=EmployeeList`, (data) => {
        $.each(data, function (i, e) {
            $('#select-employee').append(`<option value="${e.id}">${e.fullName} - ${e.number}</option>`);
        });
    }).then(function () {
        if (employee_id != 0) {
            $('#select-employee').val(employee_id);
        }
    });
}

//#endregion

//#region Hover Effects

$(function () {
    $(document).on('mouseenter', function (e) {
        var target = e.target;
        if (target.nodeName === 'BODY') {
            $('.button-list').css('visibility', 'hidden');
            $('.move-list').css('display', 'none');
        }
    });
    $(document).on('mouseleave', function (e) {
        var target = e.target;
        if (target.nodeName === 'BODY') {
            $('.button-list').css('visibility', 'hidden');
            $('.move-list').css('display', 'none');
        }
    });
    //HOVER ADD
    $(document).on('mouseenter', '.btn-add', function (e) {
        var id = $(this).val();
        $('#ul-' + id).addClass('active');
    });
    $(document).on('mouseleave', '.btn-add', function (e) {
        var id = $(this).val();
        $('#ul-' + id).removeClass('active');
    });
    //HOVER PASTE
    $(document).on('mouseenter', '.btn-paste', function (e) {
        var id = $(this).val();
        $('#ul-' + id).addClass('active');
    });
    $(document).on('mouseleave', '.btn-paste', function (e) {
        var id = $(this).val();
        $('#ul-' + id).removeClass('active');
    });
    //HOVER SHOW BUTTONS
    $(document).on('mouseenter', 'li', function (e) {
        var id = $(this).data('id');

        $('.button-list').css('visibility', 'hidden');
        $('#button-list-' + id).css('visibility', 'visible');

        $('.move-list').css('display', 'none');
        $('#move-list-' + id).css('display', 'inline-block');
    });
    $(document).on('mouseleave', 'li', function (e) {
        //$('.button-list').css('visibility', 'hidden');
    });
    //HOVER EMPLOYEE TABLE
    $(document).on('mouseenter', 'td', function () {
        var employee_id = $(this).data('employee-id');
        var hierarchy_id = $(this).data('hierarchy-id');
        $('#li-' + hierarchy_id).addClass('hover-effect');
        $('#copy-' + employee_id).css('visibility', 'visible');
    });
    $(document).on('mouseleave', 'td', function () {
        var employee_id = $(this).data('employee-id');
        $('li').removeClass('hover-effect');
        $('#copy-' + employee_id).css('visibility', 'hidden');
    });
    //HOVER MOVE UP / DOWN
    $(document).on('mouseenter', '.personnel-description , .department-description', function (e) {

        var tempp = e.target.classList[0];
        var id = $(this).data('id');
        //$('.move-list').css('display', 'none');
        //$('#move-list-' + id).css('display', 'inline-block');
    });
    $(document).on('mouseleave', '.personnel-description , .department-description', function (e) {
        //$('.move-list').css('display', 'none');
    });
});

//#endregion

//#region Collapse & Expand

$(function () {
    //Collapse Level
    $(document).on('click', '.btn-collapse-level', function (e) {
        var level = $(this).data('level');
        current_collapse_level = "level-" + level;
        CollapseLevel(level);
    });
    $(document).on('click', '.personnel-description', function (e) {
        var id = $(this).data('id');

        current_collapse_level = "";

        Collapse(id);      
    });
    $(document).on('click', '.department-description', function (e) {
        var id = $(this).data('id');

        current_collapse_level = "";

        Collapse(id);        
    });
    $('.btn-collapse').on('click', function (e) {
        //var target = e.currentTarget;
        //var id = $(target).data('id');
        //Collapse(id);        
    });
    $('.btn-collapse-all').on('click', function () {
        CollapseAll();
        $(this).blur();
        current_collapse_level = "";
    });
    $('.btn-expand-all').on('click', function () {
        ExpandAll();
        $(this).blur();
        current_collapse_level = "";
    });
});

function Collapse(id) {   
    $('#btn-collapse-' + id).toggleClass('show hide');
    $('#ul-' + id).toggleClass('show hide');
}
function CollapseAll() {
    $('.btn-collapse').removeClass('show');
    $('.btn-collapse').addClass('hide');

    $('ul').removeClass('show');
    $('ul').addClass('hide');

    $('.top').toggleClass('show hide');
}
function ExpandAll() {
    $('.btn-collapse').removeClass('hide');
    $('.btn-collapse').addClass('show');

    $('ul').removeClass('hide');
    $('ul').addClass('show');
}
function CollapseLevel(level) {
    CollapseAll();    

    for (var i = 1; i < level; i++) {
        var elements = $('.btn-collapse.level-' + i);

        $.each(elements, function (i, e) {
            var id = $(e).data('id');
            Collapse(id);
        });
    }
}

//#endregion

//#region Employee Search

$(function () {
    $('#input-employee-search').on('keyup', function () {
        var s_employee = $(this).val();
        var t_body = $('#table-body-employees');
        if (timeout) { clearTimeout(timeout); }
        timeout = setTimeout(function () {
            //get
            $.getJSON(`?handler=EmployeeSearch&EmployeeSearch=${s_employee}`, (data) => {
                $(t_body).empty();
                $.each(data, function (i, e) {
                    if (e.hierarchyId != 0) {
                        $(t_body).append(`<tr><td id="td-${e.id}" class="exists" data-hierarchy-id="${e.hierarchyId}" data-employee-id="${e.id}">${e.number} - ${e.fullName} - ${e.position}</td></tr>`);
                    }
                    else {
                        $(t_body).append(`<tr><td id="td-${e.id}" data-hierarchy-id="0" data-employee-id="${e.id}">${e.number} - ${e.fullName} - ${e.position} <span id="copy-${e.id}" data-employee-id="${e.id}" class="employee-table-copy">Copy</span></td></tr>`);
                    }
                });
            });
        }, 500);
    });    
});

//#endregion