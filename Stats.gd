class_name Stats extends Resource

@export var health: = 1 :
	set (value):
		health = value
		if health <= 0: NoHealth.emit()

@export var max_health: = 1

signal NoHealth()
