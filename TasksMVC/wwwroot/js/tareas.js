function addNewTask() {  
    tasksListVM.tasks.push(new listedElementTaskVM({ id: 0, title: '' }));

    $("[name=task-title]").last().focus();
}

async function handdleFocusOut(task) {
    const title = task.title();
    if (!title) {
        tasksListVM.tasks.pop();
        return;
    }

    const data = JSON.stringify(title);
    const response = await fetch(urlTasks, {
        method: 'POST',
        body: data,
        headers: {
            'Content-type': 'application/json'
        }
    });

    if (response.ok) {
        const json = await response.json();
        task.id(json.id);
    } else {
        handdleErrorsApi(response);
    }
}

async function getTasks() {
    tasksListVM.loading(true);

    const response = await fetch(urlTasks, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (!response.ok) {
        handdleErrorsApi(response);
        return;
    }

    const json = await response.json();
    tasksListVM.tasks([]);

    json.forEach(value => {
        tasksListVM.tasks.push(new listedElementTaskVM(value));
    });

    tasksListVM.loading(false);
}

async function updateTasksOrder() {
    const ids = getTasksIds();
    await sendTasksIdsToBackend(ids);

    const sortedTasks = tasksListVM.tasks.Sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tasksListVM.tasks([]);
    tasksListVM.tasks(sortedTasks);
}

function getTasksIds() {
    const ids = $("[name=task-title]").map(function () {
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function sendTasksIdsToBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTasks}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

async function handdleClickTask(task) {
    if (task.isNew()) {
        return;
    }

    const response = await fetch(`${urlTasks}/${task.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        handdleErrorsApi(reponse);
        return;
    }

    const json = await response.json();
    

    EditTaskVM.id = json.id;
    EditTaskVM.title(json.name);
    EditTaskVM.description(json.description);

    editTaskModalBootstrap.show();
}

async function handdleChangeEditTask() {
    const obj = {
        id: EditTaskVM.id,
        title: EditTaskVM.title(),
        description: EditTaskVM.description()
    };

    if (!obj.title) {
        return;
    }

    await editWholeTask(obj);

    const index = tasksListVM.tasks().findIndex(t => t.id() === obj.id);
    const task = tasksListVM.tasks()[index];
    task.title(obj.title);
}

async function editWholeTask(task){
    const data = JSON.stringify(task);

    const response = await fetch(`${urlTasks}/${task.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        handdleErrorsApi(reponse);
        throw "error";
    }
}

function tryToEraseTask(task) {
    editTaskModalBootstrap.hide();

    confirmAction({
        callbackAccept: () => {
            deleteTask(task);
        },
        callbackCancel: () => {
            editTaskModalBootstrap.show();
        },
        title: `¿Desea borrar la tarea ${task.title()}?`
    })
}

async function deleteTask(task) {
    const taskId = task.id;

    const response = await fetch(`${urlTasks}/${taskId}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const index = getTaskBeingEdited();
        tasksListVM.tasks.splice(index, 1);
    }
}

function getTaskBeingEdited() {
    return tasksListVM.tasks().findIndex(t => t.id() == EditTaskVM.id);
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await updateTasksOrder();
        }
    })
})