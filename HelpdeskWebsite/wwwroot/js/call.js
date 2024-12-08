$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
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

    //My call webpage is pretty buggy, uncommenting the code below causes the list to basically vanish
    //$("#CallModalForm").validate({
    //    rules: {
    //        ddlProblems: { required: true },
    //        ddlEmployees: { required: true },
    //        ddlTech: { required: true },
    //        /*labelDateOpened: { required: true },
    //        labelDateClosed: { required: true },*/
    //        TextBoxNotes: { required: true }

    //    },
    //    errorElement: "div",
    //    messages: {
    //        ddlProblems: {
    //            required: "select Problem",
    //        },
    //        ddlEmployees: {
    //            required: "select Employee",
    //        },
    //        ddlTech: {
    //            required: "select Tech",
    //        },
    //        TextBoxNotes: {
    //            required: "required 1-250 chars.",
    //        },
    //    }
    //}); //CallModalForm.validate

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
            // get problem data
            response = await fetch(`api/problem`);
            if (response.ok) {
                let divs = await response.json(); // this returns a promise, so we await it
                buildProblemList(divs);
                sessionStorage.setItem("allproblems", JSON.stringify(divs));
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
            // get technician data
            response = await fetch(`api/employee`);
            if (response.ok) {
                let divs = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(divs);
                sessionStorage.setItem("allemployees", JSON.stringify(divs));
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
            btn.html(`<div class="col-4" id="calldateopened${emp.id}">${emp.dateOpened}</div>
                      <div class="col-4" id="callEmpName${emp.id}">${emp.employeeId}</div>
                      <div class="col-4" id="callProblemDescription${emp.id}">${emp.problemId}</div>`
            );
            btn.appendTo($("#callList"));
        }); // forEach
    }; // buildCallList

    const buildProblemList = async () => {
        response = await fetch('api/problem');
        if (response.ok) {
            let problems = await response.json();
            html = '';

            $('#ddlProblems').empty();
            problems.forEach((prob) => {
                html += `<option value="${prob.id}">${prob.description}</option>`
            });
            sessionStorage.setItem('allproblems', JSON.stringify(problems));
            $('#ddlProblems').append(html);
            $('#ddlProblems').valueOf(-1);
            $('#ddlProblems').val("");
        }
    }
    const buildEmployeeList = async () => {
        response = await fetch('api/employee');
        if (response.ok) {
            let employees = await response.json();
            empHtml = '';
            techHtml = '';
            $('#ddlEmployees').empty();
            $('#ddlTech').empty();
            employees.forEach((emp) => {
                if (emp.IsTech === true) {
                    techHtml += `<option value="${emp.id}">${emp.lastname}</option>`
                }
                empHtml += `<option value="${emp.id}">${emp.lastname}</option>`
            });
            $('#ddlEmployees').append(empHtml);
            $('#ddlTech').append(techHtml);
            sessionStorage.setItem('allemployees', JSON.stringify(employees));
        }
    }
    $("#dialog").hide();
    buildEmployeeList();
    buildProblemList();
    getAll(""); // first grab the data from the server

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allcalls"));
        let filtereddata = alldata.filter((emp) => emp.employeeId.match(new RegExp($("#srch").val(), 'i')));
        buildCallList(filtereddata, false);
    }); // srch keyup

    $("#checkBoxClose").on("click", () => {
        call.problemId = $("#ddlProblems").val();
        call.employeeId = $("#ddlEmployees").val();
        call.techId = $("#ddlTech").val();
        call.notes = $("#TextBoxNotes").val();
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
        $("#DateCloseRow").hide();
        $("#CloseCallRow").hide();
        sessionStorage.removeItem("call");
        $("#uploadstatus").text("");
        $("#theModal").modal("toggle");
        let validator = $("#CallModalForm").validate();
        validator.resetForm();
        $("#status").attr("class", "");
        if (!call.openStatus) { // call is closed
            $('#ddlProblems').attr('disabled', true);
            $('#ddlEmployees').attr('disabled', true);
            $('#ddlTech').attr('disabled', true);
            $('#checkBoxClose').attr('disabled', true);
        }
    }; // clearModalFields
    const formatDate = (date) => {
        let d = new Date(date);
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
    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add call</h4>");
        $("#theModal").modal("toggle");
        $("#status").text("add new call");
        $("#theModalLabel").text("Add");
        $("#deletebutton").hide();
        $("#closedDateLabel").hide();
        $("#checkBoxClosed").hide();
        clearModalFields();
        $("#labelDateOpened").text(formatDate().replace("T", " "));
        sessionStorage.setItem("dateOpened", formatDate());
    }; // setupForAdd
    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>update call</h4>");
        $("#deletebutton").show();
        $("#closedDateLabel").show();
        $("#checkBoxClosed").show();
        clearModalFields();
        data.forEach(call => {
            if (call.id === parseInt(id)) {
                $("#ddlProblems").val(call.problemId);
                $("#ddlTech").val(call.techId);
                $("#ddlEmployees").val(call.employeeId);
                sessionStorage.setItem("dateOpened", formatDate(call.dateOpened));
                $("#labelDateOpened").text(formatDate(call.dateOpened).replace("T", " "));
                $("#TextBoxNotes").val(call.notes);
                sessionStorage.setItem("allcalls", JSON.stringify(call));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
            } // if
        }); // data.forEach
    }; // setupForUpdate

    //make tbe modal pop up when clicking on a call's details
    $("#callList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "callList" || id === "") {
            id = e.target.id;
        } // clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allcalls"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false; // ignore if they clicked on heading or status
        }
    }); // employeeListClick

    //add method
    const add = async () => {
        try {
            emp = new Object();
            emp.notes = $("#TextBoxNotes").val();
            emp.problemId = parseInt($("#ddlProblems").val());
            emp.employeeId = parseInt($("#ddlEmployees").val());
            emp.techId = parseInt($("#ddlTech").val());
            emp.dateOpened = $("#labelDateOpened").text();
            emp.dateClosed = null;
            emp.id = -1;
            emp.timer = null;
            // send the student info to the server asynchronously using POST
            let response = await fetch("api/call", {
                method: "PUT",
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

    //update method
    const update = async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of Call
            let emp = JSON.parse(sessionStorage.getItem("allcalls"));
            // pouplate the properties
            emp.notes = $("#TextBoxNotes").val();
            emp.problemId = parseInt($("#ddlProblems").val());
            emp.employeeId = parseInt($("#ddlEmployees").val());
            emp.techId = parseInt($("#ddlTech").val());
            emp.dateOpened = $("#labelDateOpened").text();
            emp.dateClosed = null;
            emp.id = -1;
            emp.timer = null;
            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("api/call", {
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
    }; // update
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

    //problem drop down list
    const loadProblemDDL = (studiv) => {
        probHtml = '';
        $('#ddlProblems').empty();
        let allproblems = JSON.parse(sessionStorage.getItem('allproblems'));
        allproblems.forEach((div) => { probHtml += `<option value="${div.id}">${div.Description}</option>` });
        $('#ddlProblems').append(probHtml);
        $('#ddlProblems').val(studiv);
    }; // loadProblemDDL

    //employee drop down list
    const loadEmployeeDDL = (studiv) => {
        empHtml = '';
        $('#ddlEmployees').empty();
        let allemployees = JSON.parse(sessionStorage.getItem('allemployees'));
        allemployees.forEach((div) => { empHtml += `<option value="${div.id}">${div.lastname}</option>` });
        $('#ddlEmployees').append(empHtml);
        $('#ddlEmployees').val(studiv);
    }; // loadEmployeeDDL

    //technician drop down list
    const loadTechnicianDDL = (studiv) => {
        techHtml = '';
        $('#ddlTech').empty();
        let allemployees = JSON.parse(sessionStorage.getItem('allemployees'));
        allemployees.forEach((div) => {
            /*if (div.IsTech === true) {
                techHtml += `<option value="${div.id}">${div.lastname}</option>`
            }*/
            techHtml += `<option value="${div.id}">${div.lastname}</option>`
        });
        $('#ddlTech').append(techHtml);
        $('#ddlTech').val(studiv);
    }; // loadTechnicianDDL
}); // jQuery ready method