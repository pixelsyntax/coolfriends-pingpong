#include "raylib.h"
#include <stdlib.h>
#include "bullets.h"

/* */

typedef struct bullet bullet_t;
struct bullet {
	Vector2 pos;
	Vector2 dir;
	float speed;
	float life_time;
	bullet_t *next;
};

bullet_t *bullets = NULL;

/* private */

void bullets_remove(bullet_t *blt) {
	if (bullets == NULL) {
		/* do_nothing */
	}
	else if (bullets == blt) {
		bullet_t *next = bullets->next;
		free(bullets);
		bullets = next; /* removed the first bullet */
	}
	else {
		bullet_t *cur = bullets;
		while (cur->next != NULL) {
			if (cur->next == blt) {
				bullet_t *next = cur->next->next;
				free(cur->next);
				cur->next = next;
				return; /* removed the bullet in between */
			}
			cur = cur->next;
		}
	}
}

/* public */

void bullets_add(Vector2 pos, Vector2 dir) {
	bullet_t *new = (bullet_t*)malloc(sizeof(bullet_t));
	new->pos = pos;
	new->dir = dir;
	new->speed = 500.0f; // TODO combine speed and dir?
	new->life_time = 1.0f;
	new->next = NULL;
	/* add new bullet to list */
	if (bullets == NULL) {
		bullets = new;
	}
	else {
		bullet_t *cur = bullets;
		while (cur->next != NULL) { cur = cur->next; }
		cur->next = new;
	}
}

void bullets_update(float dt) {
	bullet_t *cur = bullets;
	while (cur != NULL) {
		cur->pos.x += cur->dir.x * cur->speed * dt;
		cur->pos.y += cur->dir.y * cur->speed * dt;
		cur->life_time -= dt;
		bullet_t *prev = cur;
		cur = cur->next;
		if (prev->life_time <= 0.0f) {
			bullets_remove(prev);
		}
	}
}

void bullets_draw() {
	if (bullets == NULL) { return; }
	bullet_t *cur = bullets;
	for(;;) {
		DrawCircle((int)cur->pos.x, (int)cur->pos.y, 5.0f, RED);
		if (cur->next == NULL) { break; }
		cur = cur->next;
	}
}