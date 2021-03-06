﻿import { enableRipple } from '@syncfusion/ej2-base';
import { DataManager, UrlAdaptor } from '@syncfusion/ej2-data';
import { DropDownList } from '@syncfusion/ej2-dropdowns';
import { Filter, Grid, Page, Pager, QueryCellInfoEventArgs, RowDataBoundEventArgs, Sort, SortEventArgs, Toolbar, ExcelExport, Group, FilterType, Resize, ColumnChooser, Edit, ColumnMenu  } from '@syncfusion/ej2-grids';
import { Dialog } from '@syncfusion/ej2-popups';
import { ClickEventArgs } from '@syncfusion/ej2-navigations';
enableRipple(true);
Grid.Inject(Sort, Page, Filter, Toolbar, ExcelExport, Group, Resize, ColumnChooser, Edit, ColumnMenu );
let progressModel: HTMLInputElement = document.getElementById('progressDialogModal') as HTMLInputElement;

let projectId = $("#projectId").val();
let templatedata: DataManager = new DataManager({
    adaptor: new UrlAdaptor(),
    crossDomain: true,
    requestType: 'GET',
    url: '/storieslist/' + projectId,
});

let statusElem: HTMLElement;
let statusObj: DropDownList;
let status: { [key: string]: Object }[] = [
    { statusName: 'Open', statusId: '1' },
    { statusName: 'In Progress', statusId: '2' },
    { statusName: 'Hold', statusId: '3' },
    { statusName: 'Closed', statusId: '4' }
];

let storiesList: Grid = new Grid({
    actionBegin: actionBegin,
    actionComplete: actionComplete, 
    allowExcelExport: true,
    allowPaging: false,
    allowGrouping: true, 
    allowSorting: true,
    allowFiltering: true,
    allowResizing: true,
    allowTextWrap: true,
    filterSettings: { type: 'CheckBox' },
    toolbar: ['ExcelExport', 'Search', 'ColumnChooser', 'Add', 'Edit', 'Delete', 'Update', 'Cancel'],
    showColumnChooser: true,
    showColumnMenu: true,
    //enablePersistence: true,
    editSettings: { allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' },
    groupSettings: { showDropArea: true },
    columns: [
        { field: 'SortOrder', headerText: 'Sort Order', type: "number", visible: false, clipMode: 'EllipsisWithTooltip'},
        { field: 'StoryId', headerText: 'Story ID', showInColumnChooser: false, isPrimaryKey: true, type: "number", visible: false, clipMode: 'EllipsisWithTooltip' },
        { field: 'TaskId', headerText: 'Task ID', type: "number", visible: false, clipMode: 'EllipsisWithTooltip'},
        { field: 'Title', headerText: 'Title', width: '150px', type: "string", validationRules: { required: true } ,clipMode: 'EllipsisWithTooltip'},
        { field: 'ThemeName', headerText: 'Theme', type: "string", clipMode: 'EllipsisWithTooltip' },
        { field: 'EpicName', headerText: 'Epic', type: "string", clipMode: 'EllipsisWithTooltip' },
        { field: 'StoryPoints', headerText: 'Estimate', type: "number", clipMode: 'EllipsisWithTooltip' },
        { field: 'Milestone', headerText: 'Milestone', type: "string", visible: false, clipMode: 'EllipsisWithTooltip' },        
       { field: 'Benifit', headerText: 'Benefit', type: "number", visible: true, clipMode: 'EllipsisWithTooltip' },
        { field: 'Penalty', headerText: 'Penalty', type: "number", visible: true, clipMode: 'EllipsisWithTooltip' },
        { field: 'Priority', headerText: 'Priority', type: "string", visible: true, clipMode: 'EllipsisWithTooltip' },
        { field: 'Release', headerText: 'Release', type: "string", clipMode: 'EllipsisWithTooltip' },
        {
            field: 'Status', headerText: 'Status', type: "string", width: 150, visible: true, edit: {
                create: () => {
                    statusElem = document.createElement('input');
                    return statusElem;
                },
                read: () => {
                    return statusObj.text;
                },
                destroy: () => {
                    statusObj.destroy();
                },
                write: () => {
                    statusObj = new DropDownList({
                        dataSource: status,
                        fields: { value: 'statusId', text: 'statusName' },
                        placeholder: 'Select status',
                        floatLabelType: 'Never'
                    });
                    statusObj.appendTo(statusElem);
                }
            }
        },
        { field: 'SprintName', headerText: 'Sprint', type: "string", visible: false, clipMode: 'EllipsisWithTooltip' },
        { field: 'AssigneeName', headerText: 'Assignee', type: "string", visible: false, clipMode: 'EllipsisWithTooltip' },
        { field: 'Tag', headerText: 'Label', type: "string", visible: false, clipMode: 'EllipsisWithTooltip' }
    ],
    created: create,
    dataSource: templatedata,
    load: load,
    //pageSettings: { pageSize: 10 },

});
storiesList.appendTo('#storiesList');

storiesList.toolbarClick = (args: ClickEventArgs) => {
    if (args.item.id === 'storiesList_excelexport') {
        storiesList.excelExport();
    } 
};


function load(): void {
    progressModel.style.cssText = "display : block";
}

function create(): void {
    progressModel.style.cssText = "display : none";
}

function actionBegin(args): void {
    progressModel.style.cssText = "display : block";
    if (args.requestType == "save") {
        $.ajax({
            data: {
                'data': JSON.stringify(args.data),
                'projectId': projectId,
            },
            dataType: 'json',
            timeout: 180000,
            complete: function () {
                progressModel.style.cssText = "display : none";
            },
            type: "POST",
            url: '/story/addupdate/',
            error: function (response) {
                progressModel.style.cssText = "display : none";
                toastr.error("Unexpected error occured");
            },
            success: function (response) {
                storiesList.refresh();
                progressModel.style.cssText = "display : none";
                if (response.status === true) {
                    toastr.success(response.message);
                }
                else {
                    toastr.error(response.message);
                }
            },
        });
    }
    else if (args.requestType == "delete") {
        var confirm = (<any>window).confirm("Are you sure you want to delete the selected story? Once deleted it cannot be recovered.");
        if (!confirm)
            return false;

        $.ajax({
            data: {
                'storyId': args.data[0].StoryId,
            },
            dataType: 'json',
            timeout: 180000,
            complete: function () {
                progressModel.style.cssText = "display : none";
            },
            type: "POST",
            url: '/story/delete/',
            error: function (response) {
                progressModel.style.cssText = "display : none";
                toastr.error("Unexpected error occured");
            },
            success: function (response) {
                storiesList.refresh();
                progressModel.style.cssText = "display : none";
                if (response.status === true) {
                    toastr.success(response.message);
                }
                else {
                    toastr.error(response.message);
                }
            },
        });
    }
}

function actionComplete(): void {
    progressModel.style.cssText = "display : none";
}