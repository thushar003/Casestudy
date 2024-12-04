$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
const formatDate = (date) => {
    let d;
    (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
    let _day = d.getDate();
    if (_day < 10) { _day = "0" + _day; }
    let _month = d.getMonth() + 1;
    if (_month < 10) { _month = "0" + _month; }
    let _year = d.getFullYear();
    let _hour = d.getHours();
    if (_hour < 10) { _hour = "0" + _hour; }
    let _min = d.getMinutes();
    if (_min < 10) { _min = "0" + _min; }
    return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate

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
        allproblems.forEach((div) => { html += `<option value="${div.id}">${div.name}</option>` });
        $('#ddlTech').append(html);
        $('#ddlTech').val(studiv);
    }; // loadTechnicianDDL
}); // jQuery ready method