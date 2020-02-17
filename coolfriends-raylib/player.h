#ifndef _PLAYER_H
#define _PLAYER_H

#include "raylib.h"

typedef struct {
	int x;
	int y;
} player_t;

void player_input();
void player_update(player_t *player);

#endif
