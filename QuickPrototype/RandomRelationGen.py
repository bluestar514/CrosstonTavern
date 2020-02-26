#Random Relationship Gen

import random

people = ["Alicia", "Bob", "Clara", "Marco", "Daud", "Raella"]
relationAxies = ["friendship", "romance"]

relationTags={
	"aquantinces":{"friendship":(-3,5),
					"romance":(-3, 3)},
	"friends":{"friendship": (5, 10),
				"romance": (-10, 3)},
	"interested":{"friendship":(3, 10),
					"romance":(5, 10)},
	"soulmates":{"friendship":(6, 10),
					"romance": (6, 10)},
	"enemies":{"friendship":(-5, 0),
					"romance":(-10, 0)},
	"mortalEnemies":{"friendship":(-10, -5),
						"romance":(-10, 0)},
	"self":{"friendship":(0,1),
						"romance":(0,1)}
}

def pickRelationTag():
	relationTags = ["aquantinces", "aquantinces", "aquantinces","aquantinces",
		"friends","friends","friends",
		"interested", "interested",
		"soulmates", "enemies", "mortalEnemies"]
	return random.choice(relationTags)

def pickValue(tag, relation):
	ran = relationTags[tag][relation]
	return random.randrange(ran[0], ran[1])

relationships = {}
for a in people:
	relationships[a] = {}
	for b in people:
		if a==b:
			tag= "self"
		elif b in relationships and a in relationships[b]:
			tag = relationships[b][a]["tag"]
		else:
			tag = pickRelationTag()

		relationships[a][b] = {"tag":tag}
		for relation in relationAxies:
			relationships[a][b][relation] = pickValue(tag, relation)

print("\t"+ "\t".join(people))
for a in people:
	row = [a]
	for b in people:
		relation = relationships[a][b]
		tag = relation["tag"]
		friend = relation["friendship"]
		rom = relation["romance"]
		row.append("%s(%s,%s)"%(tag, friend, rom))
	print("\t".join(row))