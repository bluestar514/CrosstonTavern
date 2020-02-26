#Interface.py
#https://pysimplegui.readthedocs.io/en/latest/

import PySimpleGUI as sg    


ResourcesToActions = {
	"fish": ["fish", "trash"],
	"forage": ["mushroom", "herb"]
}


def sampleWindow():
	sg.theme('DarkAmber')    # Remove line if you want plain gray windows

	layout = [[sg.Text('Persistent window')],      
			  [sg.Input(key='-Check?-')],      
			  [sg.Button('Read'), sg.Exit()]]      

	window = sg.Window('Window that stays open', layout)      

	while True:                             # The Event Loop
		event, values = window.read() 
		print(event, values)       
		if event in (None, 'Exit'):      
			break      

	window.close()

def getListOfActions():
	return ["fish", "forage"]

def getListOfResources():
	return ["fish", "trash", "mushroom", "herb"]

def setVisablity(actions, index):
	for resource in getListOfResources():
		window["%s_%s_frame"%(index, resource)].update(visible=False)
		window["%s_%s_frame"%(index, resource)].hide_row()

	for action in actions:
		for resource in ResourcesToActions[action]:
			window["%s_%s_frame"%(index, resource)].update(visible=True)
			window["%s_%s_frame"%(index, resource)].unhide_row()

def makeFeature(ID):
	

	availableActions = sg.Listbox(	key='%s_availableActions'%ID, 
									values=getListOfActions(),
									size=(30, 6),
									select_mode=sg.LISTBOX_SELECT_MODE_MULTIPLE, 
									enable_events=True)


	layout = [
				[sg.Input(key='%s_id'%ID)],
				[availableActions]
			]

	for resource in getListOfResources():
		layout.append(makeResource(ID, resource))

	return [[	sg.Frame('Feature',
				layout,
				key="%s_feature"%ID)
			]]
	  
def makeResource(index, name):
	layout = [	sg.Frame(name,
				[
					[sg.Input(key="%s_resource_%s"%(index, name))]
				],
				key="%s_%s_frame"%(index, name),
				visible=False)
			]

	return layout

def makeWindow():
	sg.theme('DarkAmber')

	layout = [	
				[sg.Frame("Features:", makeFeature("1"), key="Features")],
				[sg.Button('AddRow'), sg.Button('JSON'), sg.Exit()]
			]

	return sg.Window('Window that stays open', layout)


def makeJSON(values):
	data = {}

	for value in values.keys():
		print(value)

		identifier = value.split("_")
		index = identifier[0]
		feild = identifier[1]

		if len(identifier) <= 2:
			if index in data:
				data[index][feild] = values[value]
			else:
				data[index] = {feild: values[value]}

		else:
			resource = identifier[2]



			if index in data:
				if feild in data[index]:
					data[index][feild][resource] = values[value]
				else:
					data[index][feild] = {
											resource: values[value]
											}
			else:
				data[index] = {feild: {
										resource: values[value]
										}
								}

	return data

window = makeWindow()

i=1

while True: 
	event, values = window.read() 
	print(event, values)    
	if event == "AddRow":
		i+=1 
		window.extend_layout(window["Features"], makeFeature(i))
	if event.find("availableActions") != -1: 
		featureIndex = event[0:event.find("_")]
		setVisablity(values[event], featureIndex)
	if event == "JSON":
		print(makeJSON(values))

	if event in (None, 'Exit'):      
		break 