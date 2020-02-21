#ifndef _BULLETS_H
#define _BULLETS_H

#include "raylib.h"


void bullets_add(Vector2 pos, Vector2 dir);
void bullets_update(float dt);
void bullets_draw();

#endif
