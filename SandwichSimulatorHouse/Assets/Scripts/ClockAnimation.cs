using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockAnimation : MonoBehaviour {

	/* 
	 * hours handle needs to rotate 360 degrees every 12 hours while
	 * minutes and seconds handle needs to rotate 360 degrees every 60 
	 * minutes and seconds respectively. These are the respective
	 * conversions for each clock handle
	 */

	const float hoursDegreesConversion = 360f / 12;
	const float minutesDegreesConversion = 360f / 60;
	const float secondsDegreesConversion = 360f / 60;

	// reference the hour, minutes and seconds hand
	public Transform hours, minutes, seconds;

	void Update () {
		// get the current time and rotate each hand accordingly
		DateTime currentTime = DateTime.Now;

		hours.rotation = Quaternion.Euler(0f, 0f, currentTime.Hour * hoursDegreesConversion);
		minutes.rotation = Quaternion.Euler(0f, 0f, currentTime.Minute * minutesDegreesConversion);
		seconds.rotation = Quaternion.Euler(0f, 0f, currentTime.Second * secondsDegreesConversion);
	}
}
