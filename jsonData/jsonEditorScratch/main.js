function test() {
	buildLocationData({"resources": {"fish":["trout", "goldfish"]}});
}

function getAllLocationData(){
	var entries = document.getElementsByClassName("form-group");

	var length = entries.length;
	for(var i = 0; i < length; i++) {
	    entry = entries[i];
	    var data = getLocationData(entry);

	    console.log(data);
	}
}

function getLocationData(entry){
	var data = {}
	var containers = entry.getElementsByClassName("container")
	
	for(var j = 0; j< containers.length; j++){
		var container = containers[j];

		var newData = {}
		if(container.id == "id_holder"){
			newData = getId(container);
		}else if(container.id == "connected_locations_holder"){
			newData = getConnectedLocations(container);
		}else if(container.id == "resources_holder"){
			newData = getResources(container);
		}

		Object.assign(data, newData);
	}
	return data;
}

function getId(container){
	var input = container.getElementsByClassName("form-control")[0];

	var data = {};
	data[input.name] = input.value;

	return data;
}

function getConnectedLocations(container){
	var selectors = container.getElementsByClassName("custom-select");
	data = {}
	for(var j = 0; j< selectors.length; j++){
		var point = selectors[j]

		var key = point.name;
		var value = point.value;

		if(key in data){
			data[key].push(value);
		}else{
			data[key] = [value];
		}
	}

	return data;
}

function getResources(container){
	data = {}

	var rows = container.getElementsByClassName("elementRow");
	data = {}
	for(var j = 0; j< rows.length; j++){
		var row = rows[j]
		
		var cols = row.getElementsByClassName("form-control");
		var key = "";
		var array = [];

		for(var k=0; k<cols.length; k++){
			var point = cols[k];

			var name = point.name;
			var value = point.value;

			if(name == "resourceKey"){
				key = value;
			}
			if(name == "resourceValue"){
				array.push(value);
			}
		}
		data[key] = array;
	}

	return data;
}


function buildLocationData(data){
	var locationCard = document.getElementById("locationCard");

	var newCard = locationCard.cloneNode(true);
	newCard.hidden = false;

	var locationHolder = document.getElementById("editor_holder");

	buildResourceEntry(data["resources"], newCard);

	locationHolder.appendChild(newCard);
}

function buildResourceEntry(data, card){
	var resourceRow = document.getElementById("resourceRow");

	for(key in data){
		var newRow = resourceRow.cloneNode(true);
		newRow.hidden = false;
		var keyInput = newRow.getElementsByClassName("form-control")[0];

		keyInput.value = key;

		for(value in data[key]){
			buildResourceValue(data[key][value], newRow);
		}

		var addValueButton = newRow.getElementsByClassName("addValue")[0];
		addValueButton.addEventListener("click", function(){
			buildResourceValue("", newRow);
		});

		var resourceHolder = card.getElementsByClassName("resourceHolder")[0];
		resourceHolder.appendChild(newRow);
	}
}

function buildResourceValue(value, resourceRow){
	console.log("test");
	var resourceValue = document.getElementById("valueRow");
	var newValue = resourceValue.cloneNode(true);
	newValue.hidden = false;
	var valueInput = newValue.getElementsByClassName("form-control")[0];

	valueInput.value = value;

	var addValueButton = newValue.getElementsByClassName("deleteValue")[0];
	addValueButton.addEventListener("click", function(){
		deleteElement(newValue);
	});

	var holder = resourceRow.getElementsByClassName("valueHolder")[0];
	holder.appendChild(valueInput);
}

function deleteElement(element){
	element.remove();
}