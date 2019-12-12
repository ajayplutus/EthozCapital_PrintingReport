$(function () {
    $("#grid").jqGrid({
        url: "/TodoList/GetTodoLists",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['Id', 'Task Name', 'Task Description', 'Severity', 'Task Status'],        
        colModel: [
            { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
            { key: false, name: 'TaskName', index: 'TaskName', editable: true },
            { key: false, name: 'TaskDescription', index: 'TaskDescription', editable: true },            
            { key: false, name: 'Severity', index: 'Severity', editable: true, edittype: 'select', editoptions: { value: { 'L': 'Low', 'M': 'Medium', 'H': 'High' } } },
            { key: false, name: 'TaskStatus', index: 'TaskStatus', editable: true, edittype: 'select', editoptions: { value: { 'A': 'Active', 'I': 'InActive' } } }],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Todo List',
        emptyrecords: 'No records to display',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#pager', { edit: true, add: true, del: true, search: false, refresh: true },
        {
            // edit options
            zIndex: 100,
            url: '/TodoList/Edit',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    //alert(response.responseText);
                }
            }
        },
        {
            // add options
            zIndex: 100,
            url: "/TodoList/Create",
            closeOnEscape: true,
            closeAfterAdd: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    //alert(response.responseText);
                }
            }
        },
        {
            // delete options
            zIndex: 100,
            url: "/TodoList/Delete",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete this task?",
            afterComplete: function (response) {
                if (response.responseText) {
                    //alert(response.responseText);
                }
            }
        });
});

$(window).on('resize.jqGrid', function () {
    $("#grid").jqGrid('setGridWidth', $("#content").width());
})