$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    /*document.addEventListener("keyup",
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
        });*/

    /*$("#CallModalForm").validate({
        rules: {
            ddlProblems: { required: true },
            ddlEmployees: { required: true },
            ddlTech: { required: true },
            TextBoxNotes: { required: true, maxlength: 250 }
        },
        errorElement: "div",
        messages: {
            ddlProblems: {
                required: "Please select a problem."
            },
            ddlEmployees: {
                required: "Please select an employee."
            },
            ddlTech: {
                required: "Please select a technician."
            },
            TextBoxNotes: {
                required: "Notes are required (1-250 characters).",
                maxlength: "Notes cannot exceed 250 characters."
            }
        }
    }); //CallModalForm.validate*/

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
                      <div class="col-4" id="callemployeename{emp.id}">${emp.employeeName}</div>
                      <div class="col-4" id="callproblemdescription{emp.id}">${emp.problemDescription}</div>`
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
            problems.forEach((prob) => { html += `<option value="${prob.id}">${prob.description}</option>` });
            sessionStorage.setItem('allproblems', JSON.stringify(problems));
            $('#ddlProblems').append(html);
            $('#ddlProblems').val(-1);
            $('#ddlProblems').val("");
        }
    }
    
    const buildEmployeeList = async () => {
        response = await fetch('api/employee');
        if (response.ok) {
            let employees = await response.json();
            let empHtml = '';
            let techHtml = '';
            let technicians = []; // Empty array

            $('#ddlEmployees').empty();
            $('#ddlTech').empty();

            employees.forEach((emp) => {
                empHtml += `<option value="${emp.id}">${emp.lastname}</option>`;
                if (emp.IsTech === true) {
                    techHtml += `<option value="${emp.id}">${emp.lastname}</option>`;
                    technicians.push(emp); // Add technician to the technicians array
                }
            });

            // Save full list and technicians list to session storage
            sessionStorage.setItem('allemployees', JSON.stringify(employees));
            sessionStorage.setItem('alltechnicians', JSON.stringify(technicians));

            // Populate dropdowns
            $('#ddlEmployees').append(empHtml);
            $('#ddlTech').append(techHtml);
        }
    };
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
       if ($("#checkBoxClose").is(":checked")) {
           const currentDate = formatDate(new Date());
           $("#labelDateClosed").text(currentDate.replace("T", " "));
           sessionStorage.setItem("dateClosed", currentDate);
       } else {
           $("#labelDateClosed").text("");
           sessionStorage.setItem("dateClosed", "");
       }
   });

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
       $("#status").attr("class", "");
       $("#ddlProblems").attr("disabled", false);
       $("#ddlEmployees").attr("disabled", false);
       $("#ddlTech").attr("disabled", false);
       $("#checkBoxClose").attr("disabled", false);
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
        $("#modaltitle").html("<h4>Add Call</h4>");
        $("#theModal").modal("toggle");
        $("#status").text("Add new call");
        $("#theModalLabel").text("Add");
        $("#deletebutton").hide();
        $("#closedDateLabel").hide();
        $("#checkBoxClosed").hide();
        clearModalFields();

        // Set current date as dateOpened
        const currentDate = formatDate(new Date());
        $("#labelDateOpened").text(currentDate.replace("T", " "));
        sessionStorage.setItem("dateOpened", currentDate);
    };

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>Update Call</h4>");
        $("#deletebutton").show();
        $("#closedDateLabel").show();
        $("#checkBoxClosed").show();
        //clearModalFields();

        const call = data.find(call => call.id === parseInt(id));
        if (call) {
            // Set Problem dropdown value based on problemId
            $("#ddlProblems").val(call.problemId);
            // Set Technician dropdown value based on techId
            $("#ddlTech").val(call.techId);
            // Set Employee dropdown value based on employeeId
            $("#ddlEmployees").val(call.employeeId);
            const formattedDateOpened = formatDate(call.dateOpened);
            $("#labelDateOpened").text(formattedDateOpened.replace("T", " "));
            sessionStorage.setItem("dateOpened", formattedDateOpened);

            if (call.dateClosed) {
                const formattedDateClosed = formatDate(call.dateClosed);
                $("#labelDateClosed").text(formattedDateClosed.replace("T", " "));
                sessionStorage.setItem("dateClosed", formattedDateClosed);
                $("#checkBoxClose").prop("checked", true);
            } else {
                $("#labelDateClosed").text("");
                sessionStorage.setItem("dateClosed", "");
                $("#checkBoxClose").prop("checked", false);
            }

            $("#TextBoxNotes").val(call.notes);
            sessionStorage.setItem("call", JSON.stringify(call));
            // Update modal status
            $("#modalstatus").text("Update data");
            $("#theModal").modal("toggle");
            $("#theModalLabel").text("Update");
        }
    };

    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "update" ? update() : add();
    }); // actionbutton click

    //make the modal pop up when clicking on a call's details
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
            /*emp.id = -1;
            emp.timer = null;*/
            sessionStorage.setItem("call", JSON.stringify(emp));
            // send the call info to the server asynchronously using POST
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

    //update method
    const update = async () => {
        try {
            // Retrieve current call object from sessionStorage
            let call = JSON.parse(sessionStorage.getItem("call"));
            if (!call) {
                $("#status").text("No call data available for update.");
                return;
            }

            // Populate call object with updated data from the modal
            call.notes = $("#TextBoxNotes").val();
            call.problemId = parseInt($("#ddlProblems").val());
            call.employeeId = parseInt($("#ddlEmployees").val());
            call.techId = parseInt($("#ddlTech").val());
            call.dateOpened = sessionStorage.getItem("dateOpened");
            call.dateClosed = sessionStorage.getItem("dateClosed") || null;

            // Send updated call back using PUT
            let response = await fetch(`api/call/${call.id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(call)
            });

            if (response.ok) {
                let data = await response.json();
                getAll(data.msg); // Refresh the call list with updated data
                $("#theModal").modal("toggle");
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("No such path on the server.");
            }
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        }
    };

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
        allproblems.forEach((div) => { probHtml += `<option value="${div.id}">${div.description}</option>` });
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
    const loadTechnicianDDL = (selectedTechId) => {
        techHtml = '';
        $('#ddlTech').empty();

        // Get technicians from session storage
        let alltechnicians = JSON.parse(sessionStorage.getItem('allemployees'));
        alltechnicians.forEach((tech) => {
            techHtml += `<option value="${tech.id}">${tech.lastname}</option>`;
        });

        // Populate dropdown and set the selected value
        $('#ddlTech').append(techHtml);
        $('#ddlTech').val(selectedTechId || -1); // Default val: -1
    };
}); // jQuery ready method