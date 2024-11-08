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
        // Mostrar mensaje de error
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
        return;
    }

    const json = await response.json();
    tasksListVM.tasks([]);

    json.forEach(value => {
        tasksListVM.tasks.push(new listedElementTaskVM(value));
    });

    tasksListVM.loading(false);
}