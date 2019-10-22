static func repeat(t:float, length:float):
	return clamp(t - floor(t / length) * length, 0.0, length)

static func delta_angle(current:float, target:float):
	var delta = repeat(target - current, PI * 2.0)
	if delta > PI:
		delta -= PI * 2.0
	return delta
		
static func smooth_damp(current:float, target:float, currentVelocity:Array, smoothTime:float, maxSpeed:float, deltaTime:float):
	 var num = 2.0 / smoothTime
	 var num2 = num * deltaTime
	 var num3 = 1.0 / (1.0 + num2 + 0.48 * num2 * num2 + 0.235 * num2 * num2 * num2)
	 var num4 = current - target
	 var num5 = target
	 var num6 = maxSpeed * smoothTime
	 num4 = clamp(num4, -num6, num6)
	 target = current - num4
	 var num7 = (currentVelocity[0] + num * num4) * deltaTime
	 currentVelocity[0] = (currentVelocity[0] - num * num7) * num3
	 var num8 = target + (num4 + num7) * num3
	 if (num5 - current > 0.0) == (num8 > num5):
		 num8 = num5
		 currentVelocity[0] = (num8 - num5) / deltaTime
	 return num8

# https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
static func smooth_damp_angle(current:float, target:float, currentVelocity:Array, smoothTime:float, maxSpeed:float, deltaTime:float):
	target = current + delta_angle(current, target)
	return smooth_damp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime)

# https://github.com/jamesjlinden/unity-decompiled/blob/master/UnityEngine/UnityEngine/Vector2.cs
static func smooth_damp_vec2(current:Vector2, target:Vector2, currentVelocity:Array, smoothTime:float, maxSpeed:float, deltaTime:float):
	smoothTime = max(0.0001, smoothTime)
	var num1 = 2.0 / smoothTime
	var num2 = num1 * deltaTime
	var num3 = (1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2))
	var vector = current - target
	var vector2_1 = target
	var maxLength = maxSpeed * smoothTime
	var vector2_2 = vector if (vector.length() <= maxLength) else (vector.normalized() * maxLength)
	target = current - vector2_2
	var curVel = Vector2(currentVelocity[0], currentVelocity[1])
	var vector2_3 = (curVel + num1 * vector2_2) * deltaTime
	curVel = (curVel - num1 * vector2_3) * num3
	var vector2_4 = target + (vector2_2 + vector2_3) * num3
	if (vector2_1 - current).dot(vector2_4 - vector2_1) > 0.0:
		vector2_4 = vector2_1
		curVel = (vector2_4 - vector2_1) / deltaTime
	currentVelocity[0] = curVel.x
	currentVelocity[1] = curVel.y
	return vector2_4

static func move_towards(current:float, target:float, dist:float):
	var diff = target - current
	if abs(diff) < abs(dist):
		return current + diff
	return current + abs(dist) * sign(diff)
