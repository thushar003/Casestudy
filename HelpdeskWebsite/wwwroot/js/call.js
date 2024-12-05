$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) {
            _day = "0" + _day;
        }
        let _month = d.getMonth() + 1;
        if (_month < 10) {
            _month = "0" + _month;
        }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) {
            _hour = "0" + _hour;
        }
        let _min = d.getMinutes();
        if (_min < 10) {
            _min = "0" + _min;
        }
        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate

    document.addEventListener("keyup",
        e => {
            $("#modalstatus").removeClass(); //remove any existing css on div
            if ($("#CallModalForm").valid()) {
                $("#modalstatus").attr("class", "badge bg-success"); //green
                $("#modalstatus").text("data entered is valid");
                $("#actionbutton").prop("disabled", false);
            } else {
                $("#modalstatus").attr("class", "badge bg-danger"); //red
                $("#modalstatus").text("fix errors");
                $("#actionbutton").prop("disabled", true);
            }
        });
    $("#CallModalForm").validate({
        rules: {
            ddlProblems: { required: true },
            ddlEmployees: { required: true },
            ddlTech: { required: true },
            TextBoxOpenDate: { required: true },
            TextBoxCloseDate: { required: true },
            TextBoxNotes: { required: true }

        },
        errorElement: "div",
        messages: {
            ddlProblems: {
                required: "select Problem",
            },
            ddlEmployees: {
                required: "select Employee",
            },
            ddlTech: {
                required: "select Tech",
            },
            TextBoxNotes: {
                required: "required 1-250 chars.",
            },
        }
    }); //EmployeeModalForm.validate

    const getAll = async (msg) => {
        try {
            $("#callList").text("Finding Call Information...");
            let response = await fetch(`api/call`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildCallList(payload);
                msg === "" ? // are we appending to an existing message
                    $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Calls Loaded`);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else

            // get department data
            /*response = await fetch(`api/department`);
            if (response.ok) {
                let divs = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("alldepartments", JSON.stringify(divs));
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else*/
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll

    const buildCallList = (data, usealldata = true) => {
        $("#callList").empty();
        div = $(`<div class="list-group-item text-blue bg-green row d-flex text-white" id="status">Call Info</div>
<div class= "list-group-item row d-flex text-center" id="heading">
<div class="col-4 h4">Date</div>
<div class="col-4 h4">For</div>
<div class="col-4 h4">Problem</div>
</div>`);
        div.appendTo($("#callList"));
        usealldata ? sessionStorage.setItem("allcalls", JSON.stringify(data)) : null;
        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add call</button>`);
        btn.appendTo($("#callList"));
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex" id="${emp.id}">`);
            btn.html(`<div class="col-4" id="calldateopened{emp.id}">${emp.dateopened}</div>
<div class="col-4" id="callfor${emp.id}">${emp.for}</div>
<div class="col-4" id="callproblem${emp.id}">${emp.problem}</div>`
            );
            btn.appendTo($("#callList"));
        }); // forEach
    }; // buildCallList
    $("#dialog").hide();
    getAll(""); // first grab the data from the server

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allcalls"));
        let filtereddata = alldata.filter((emp) => emp.employeeId.match(new RegExp($("#srch").val(), 'i')));
        buildCallList(filtereddata, false);
    }); // srch keyup

    $("#checkBoxClose").on("click", () => {
        if ($("#checkBoxClose").is(":checked")) {
            $("#labelDateClosed").text(formatDate().replace("T", " "));
            sessionStorage.setItem("dateClosed", formatDate());
        } else {
            $("#labelDateClosed").text("");
            sessionStorage.setItem("dateClosed", "");
        }
    }); // checkBoxClose

    const clearModalFields = () => {
        loadProblemDDL(-1);
        loadTechnicianDDL(-1);
        loadEmployeeDDL(-1);
        $("#TextBoxNotes").val("");
        sessionStorage.removeItem("employee");
        $("#uploadstatus").text("");
        $("#theModal").modal("toggle");
        let validator = $("#CallModalForm").validate();
        validator.resetForm();
        $("#status").attr("class", "");
    }; // clearModalFields
    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add call</h4>");
        $("#theModal").modal("toggle");
        $("#status").text("add new call");
        $("#theModalLabel").text("Add");
        $("#deletebutton").hide();
        clearModalFields();
    }; // setupForAdd
    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>update call</h4>");
        $("#deletebutton").show();
        clearModalFields();
        data.forEach(call => {
            if (call.id === parseInt(id)) {
                $("#ddlProblems").val(call.problemId);
                $("#ddlTech").val(call.employeeId);
                $("#ddlProblems").val(employee.problemId);
                $("#TextBoxNotes").val(call.notes);
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
            } // if
        }); // data.forEach
    }; // setupForUpdate
    const add = async () => {
        try {
            emp = new Object();
            emp.notes = $("#TextBoxNotes").val();
            emp.problemId = parseInt($("#ddlProblems").val());
            emp.employeeId = parseInt($("#ddlEmployees").val());
            emp.techId = parseInt($("#ddlTech").val());
            emp.id = -1;
            emp.timer = null;
            // send the student info to the server asynchronously using POST
            let response = await fetch("api/call", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify(emp)
            });
            if (response.ok) // or check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        } // try/catch
        $("#theModal").modal("toggle");
    }; // add

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
    const loadProblemDDL = (studiv) => {
        html = '';
        $('#ddlProblems').empty();
        let allproblems = JSON.parse(sessionStorage.getItem('allproblems'));
        allproblems.forEach((div) => { html += `<option value="${div.id}">${div.name}</option>` });
        $('#ddlProblems').append(html);
        $('#ddlProblems').val(studiv);
    }; // loadProblemDDL
    const loadEmployeeDDL = (studiv) => {
        html = '';
        $('#ddlEmployees').empty();
        let allemployees = JSON.parse(sessionStorage.getItem('allemployees'));
        allemployees.forEach((div) => { html += `<option value="${div.id}">${div.name}</option>` });
        $('#ddlEmployees').append(html);
        $('#ddlEmployees').val(studiv);
    }; // loadEmployeeDDL
    const loadTechnicianDDL = (studiv) => {
        html = '';
        $('#ddlTech').empty();
        let allemployees = JSON.parse(sessionStorage.getItem('allemployees'));
        let technicians = allemployees.filter(empployee => employeeId.isTech);
        technicians.forEach((div) => { html += `<option value="${div.id}">${div.name}</option>` });
        $('#ddlTech').append(html);
        $('#ddlTech').val(studiv);
    }; // loadTechnicianDDL
}); // jQuery ready method