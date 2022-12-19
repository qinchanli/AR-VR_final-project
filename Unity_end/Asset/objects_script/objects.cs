using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// interface of all objects
interface objects {
    void move_forward();
	void move_backward();
	void move_rightward();
	void move_leftward();
	void jump();
	double velocity_func {  get ; set ;}
}
