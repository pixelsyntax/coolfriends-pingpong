#include "raylib.h"
#include <stdlib.h>
#include "player.h"

/* */

typedef enum { up, down, left, right } input_action_e;

typedef struct input_action_buffered input_action_buffered_t;
struct input_action_buffered {
	input_action_e action;
	input_action_buffered_t *next;
};

input_action_buffered_t *player_cur_action = NULL;

/* private */

void player_buffer_input(input_action_e action) {
	if (player_cur_action == NULL) {
		player_cur_action = (input_action_buffered_t*)malloc(sizeof(input_action_buffered_t));
		player_cur_action->action = action;
		player_cur_action->next = NULL;
		return; /* first currently active action */
	}
	input_action_buffered_t *cur = player_cur_action;
	for(;;) {
		if (cur->action == action) {
			if (player_cur_action == cur) {
				return; /* do nothing */
			}
			input_action_buffered_t *temp_next = cur->next;
			cur->next = player_cur_action->next;
			player_cur_action->next = temp_next;
			player_cur_action = cur;
			return; /* swapped the inputs, this one is now first */
		}
		if (cur->next == NULL) { break; }
		cur = cur->next;
	}
	/* create a new action, and make it the first one */
	input_action_buffered_t *new = (input_action_buffered_t*)malloc(sizeof(input_action_buffered_t));
	new->action = action;
	new->next = player_cur_action;
	player_cur_action = new;
}

void player_unbuffer_input(input_action_e action) {
	if (player_cur_action == NULL) {
		/* do_nothing */
	}
	else if (player_cur_action->action == action) {
		input_action_buffered_t *next = player_cur_action->next;
		free(player_cur_action);
		player_cur_action = next; /* removed the first action */
	}
	else {
		input_action_buffered_t *cur = player_cur_action;
		while (cur->next != NULL) {
			if (cur->next->action == action) {
				input_action_buffered_t *next = cur->next->next;
				free(cur->next);
				cur->next = next;
				return; /* removed the action in between */
			}
			cur = cur->next;
		}
	}
}

/* public */

void player_update(player_t *player) {
	if (player_cur_action != NULL) {
		switch (player_cur_action->action) {
			case up: player->y--; break;
			case down: player->y++; break;
			case left: player->x--; break;
			case right: player->x++; break;
		}
	}
}

void player_input() {
	if (IsKeyPressed( KEY_UP ) || IsKeyPressed( KEY_W )) { player_buffer_input( up ); }
	else if (IsKeyReleased( KEY_UP ) || IsKeyReleased( KEY_W )) { player_unbuffer_input( up ); }
	if (IsKeyPressed( KEY_DOWN ) || IsKeyPressed( KEY_S )) { player_buffer_input( down ); }
	else if (IsKeyReleased( KEY_DOWN ) || IsKeyReleased( KEY_S )) { player_unbuffer_input( down ); }
	if (IsKeyPressed( KEY_LEFT ) || IsKeyPressed( KEY_A )) { player_buffer_input( left ); }
	else if (IsKeyReleased( KEY_LEFT ) || IsKeyReleased( KEY_A )) { player_unbuffer_input( left ); }
	if (IsKeyPressed( KEY_RIGHT ) || IsKeyPressed( KEY_D )) { player_buffer_input( right ); }
	else if (IsKeyReleased( KEY_RIGHT ) || IsKeyReleased( KEY_D )) { player_unbuffer_input( right ); }
}
