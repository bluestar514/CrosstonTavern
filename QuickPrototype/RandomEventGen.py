#simple random event generator
import random


huntingResults= ["rabbit", "boar", "pigeon", "duck", 
				"bear", "wolf", "nothing"]
foragingResults = ["herb", "berry", "mushroom", "flower", "nothing"]
colors = ["red", "orange", "yellow", "green", "blue", "purple", "pink"]
fishingResults= ["fish", "algee", "jellyfish", "ray", "shark", "turtle", "nothing", "old boot"]

people = ["Alicia", "Bob", "Clara", "Marco", "Daud"]
direction = ["north", "south", "east", "west"]
locations = ["forest", "feild", "river", "lake"]

actions = {
	"hunt": huntingResults,
	"forage": foragingResults,
	"fish": fishingResults,
	"chat": people
}

def pickAction():
	return random.choice(list(actions.keys()))
def pickActor():
	return random.choice(people)
def pickResult(action):
	return random.choice(actions[action])
def pickLocation():
	return random.choice(direction) + " " + random.choice(locations)

def makeMemory(day, hour, minute, actor):
	action = pickAction()
	result = pickResult(action)
	location = pickLocation()

	print("%s, %s:%s0 - %s(%s, %s)->%s"%(day, hour, minute, action, 
											actor, location, result))


for day in range(7):
	for hour in range(8, 20):
		for minute in range(6):
			makeMemory(day, hour, minute, "Raella")

			for i in range(random.randrange(4)):
				makeMemory(day, hour, minute, pickActor())

			