
let div_chart = document.getElementById('div-chart');
let locaiton_id = document.getElementById('input-location-id');

google.charts.load('current', { packages: ['orgchart'] });
google.charts.setOnLoadCallback(drawChart);

function drawChart() {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Name');
    data.addColumn('string', 'Manager');

    $.getJSON('?handler=LocationTree', function (results) {
        $.each(results, (i, e) => {
            
            var id = e.locationId.toString();
            var parent_id = e.parentLocationId.toString();
            var user = e.user;
            if (i == 0) {
                parent_id = '';
            }
            //<img class="img" src="\\images\\${e.employee.imageUrl}" />
            //<img class="img" src="${e.employee.imageUrl}" />

            if (user != null) {
                html = `<div class="">${user.fullName}</div>`;
            }
            else {
                html = `<div class=""></div>`;
            }            

            var name =
            {
                'v': id,
                'f': html
            };

            data.addRow([name, parent_id]);            
        });
    }).then(function () {
        //Create the chart.
        var chart = new google.visualization.OrgChart(div_chart);
        //Draw the chart, setting the allowHtml option to true for the tooltips.
        //chart.ready(ChartLoaded());
        chart.draw(data, {
            'allowHtml': true,
            'allowCollapse': true,
            'size': 'small',
            //'selectedNodeClass': 'selected-employee'
        });
    });

}