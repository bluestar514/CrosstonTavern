 function loadJSON(callback) {   

    var xobj = new XMLHttpRequest();
        xobj.overrideMimeType("application/json");
    xobj.open('GET', 'townSchema.json', true); // Replace 'my_data' with the path to your file
    xobj.onreadystatechange = function () {
          if (xobj.readyState == 4 && xobj.status == "200") {
            // Required use of an anonymous callback as .open will NOT return a value but simply returns undefined in asynchronous mode
            callback(xobj.responseText);
          }
    };
    xobj.send(null);  
 }
 

loadJSON(function(response) {
  // Parse JSON string into object
    var json = JSON.parse(response);

    console.log(json);


    var editor = new JSONEditor(document.getElementById('editor_holder'),{
		schema: json,
		theme: 'bootstrap4'
	});



	// Hook up the submit button to log to the console
	document.getElementById('submit').addEventListener('click',function() {
	// Get the value from the editor
	console.log(editor.getValue());
	});
 });


