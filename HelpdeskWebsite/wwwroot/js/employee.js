$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(payload);
                msg === "" ? // are we appending to an existing message
                    $("#status").text("employees Loaded") : $("#status").text(`${msg} - employees Loaded`);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll
    const buildEmployeeList = (data) => {
        $("#employeeList").empty();
        div = $(`<div class="list-group-item text-blue bg-green row d-flex text-white" id="status">Employee Info</div>
<div class= "list-group-item row d-flex text-center" id="heading">
<div class="col-4 h4">Title</div>
<div class="col-4 h4">First</div>
<div class="col-4 h4">Last</div>
</div>`);
        div.appendTo($("#employeeList"));
        sessionStorage.setItem("allemployees", JSON.stringify(data));
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex" id="${emp.id}">`);
            btn.html(`<div class="col-4" id="employeetitle${emp.id}">${emp.title}</div>
<div class="col-4" id="employeefname${emp.id}">${emp.firstname}</div>
<div class="col-4" id="employeelastname${emp.id}">${emp.lastname}</div>`
            );
            btn.appendTo($("#employeeList"));
        }); // forEach
    }; // buildEmployeeList
    getAll(""); // first grab the data from the server

    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        } // clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            data.forEach(employee => {
                if (employee.id === parseInt(id)) {
                    $("#TextBoxPhone").val(employee.phoneno);
                    $("#TextBoxEmail").val(employee.email);
                    $("#TextBoxFirstN").val(employee.firstname);
                    $("#TextBoxLastN").val(employee.lastname);
                    $("#TextBoxT").val(employee.title);
                    sessionStorage.setItem("employee", JSON.stringify(employee));
                    $("#modalstatus").text("update data");
                    $("#theModal").modal("toggle");
                } // if
            }); // data.map
        } else {
            return false; // ignore if they clicked on heading or status
        }
    }); // employeeListClick
    $("#actionbutton").on('click', async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of employee
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            // pouplate the properties
            emp.title = $("#TextBoxT").val();
            emp.firstname = $("#TextBoxFirstN").val();
            emp.lastname = $("#TextBoxLastN").val();
            emp.email = $("TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();
            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });

            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                getAll(payload.msg);
                $("#theModal").modal("toggle");
            } else if (response.status !== 404) {
                // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                // else 404 not found
                $("#status").text("no such path on server");
            } // else

        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        } // try/catch
    }); // action button click
    const errorRtn = (problemJson, status) => {
        if (status > 499) {
            $("#status").text("Problem server side, see debug console");
        } else {
            let keys = Object.keys(problemJson.errors)
            problem = {
                status: status,
                statusText: problemJson.errors[keys[0]][0], // first error
            };
            $("#status").text("Problem client side, see browser console");
            console.log(problem);
        } // else
    }
}); // jQuery ready method