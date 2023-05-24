
let div_chart = document.getElementById('div-chart');

google.charts.load('current', { packages: ['orgchart'] });
google.charts.setOnLoadCallback(drawChart);

function drawChart() {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Name');
    data.addColumn('string', 'Manager');

    $.getJSON('?handler=EmployeeData', function (results) {
        $.each(results, (i, e) => {
            var html = '';
            var id = e.id.toString();
            var parent_id = e.parentId.toString();
            //<img class="img" src="\\images\\${e.employee.imageUrl}" />
            if (e.type == 0) {
                html = `<div class="department-name">${e.description}</div>`;
            }
            else {
                if (e.employee.id != 0) {

                    html = `<img class="img" src="${e.employee.imageUrl}" />
                        <div class="employee-position">${e.description}</div>
                        <div id="employee-${e.employee.id}" class="employee-name" data-employee-id="${e.employee.id}">${e.employee.fullName}</div>`
                }
                else {
                    html = `<img class="img" src="\\images\\${e.employee.imageUrl}" />
                            <div class="employee-position">${e.description}</div>
                            <div class="text-secondary">Not Filled</div>`
                }
            }

            var name =
            {
                'v': id,
                'f': html
            };

            data.addRow([name, parent_id]);

        });
    }).then(function () {
        // Create the chart.
        var chart = new google.visualization.OrgChart(div_chart);
        // Draw the chart, setting the allowHtml option to true for the tooltips.
        //chart.ready(ChartLoaded());
        chart.draw(data, {
            'allowHtml': true,
            'compactRows': true,
            'selectedNodeClass': 'selected-employee'
        });
    });

}

let pos = { top: 0, left: 0, x: 0, y: 0 };

const mouseDownHandler = function (e) {
    // Change the cursor and prevent user from selecting the text
    div_chart.style.cursor = 'grabbing';
    div_chart.style.userSelect = 'none';

    pos = {
        // The current scroll
        left: div_chart.scrollLeft,
        top: div_chart.scrollTop,
        // Get the current mouse position
        x: e.clientX,
        y: e.clientY,
    };

    document.addEventListener('mousemove', mouseMoveHandler);
    document.addEventListener('mouseup', mouseUpHandler);
};
const mouseMoveHandler = function (e) {
    // How far the mouse has been moved
    const dx = e.clientX - pos.x;
    const dy = e.clientY - pos.y;

    // Scroll the element
    div_chart.scrollTop = pos.top - dy;
    div_chart.scrollLeft = pos.left - dx;
};
const mouseUpHandler = function (e) {
    document.removeEventListener('mousemove', mouseMoveHandler);
    document.removeEventListener('mouseup', mouseUpHandler);

    div_chart.style.cursor = 'grab';
    div_chart.style.removeProperty('user-select');
};

div_chart.addEventListener('mousedown', mouseDownHandler);

$(document).on('click', '.google-visualization-orgchart-node', function (e) {
    var node = $('.selected-employee')[0];

    if (node != undefined) {

        var employee = $(node).find('.employee-name')[0];
        if (employee != undefined) {

            var id = $(employee).data('employee-id');
            if (id != 0 && id != undefined) {

                $.getJSON(`/Index?handler=SelectEmployee&EmployeeId=${id}`, (data) => {
                    if (data != null) {
                        console.log({data});
                        $('#span-employee-name').text(data.fullName);
                        $('#span-employee-details-2').text(data.departmentName + ' (' + data.departmentId + ')');
                        $('#span-employee-details-3').text(data.position);
                        $('#span-employee-details-4').text(data.str_HireDate);
                        $('#span-employee-details-5').text(data.number);
                        $('#span-employee-details-5').addClass('border-bottom');
                        $('#img-employee').attr('src', data.imageUrl);
                        var ad = data.ad;

                        if (ad != null) {
                            $('#span-employee-details-6').text('Username: ' + data.ad.userName);
                            $('#span-employee-details-7').text('Email: ' + data.ad.email);
                        }
                        else {
                            $('#span-employee-details-6').text('');
                            $('#span-employee-details-7').text('');
                        }
                    }
                });
            }
        }        
    }
});

$('.btn-temp').on('click', function (e) {
    var target = e.target;

    var scale = $(target).data('scale');

    console.log(scale);

    switch (scale) {
        case 1: $('.google-visualization-orgchart-table').css('scale', '1'); break;
        case 2: $('.google-visualization-orgchart-table').css('scale', '.8'); break;
        case 3: $('.google-visualization-orgchart-table').css('scale', '.6'); break;
        case 4: $('.google-visualization-orgchart-table').css('scale', '.4'); break;
        case 5: $('.google-visualization-orgchart-table').css('scale', '.2'); break;
    }
});