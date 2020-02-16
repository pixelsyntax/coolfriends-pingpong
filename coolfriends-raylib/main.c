//
//  small pingpong game that uses raylib
//  check https://www.raylib.com/cheatsheet/cheatsheet.html for API
//
//--------------------------------------------------------------------------------------

#include "raylib.h"
#include "player.h"
#include "loot.h"
#include <math.h>

int lerp(int a, int b, float d) {
    return roundf(a * (1 - d) + b * d);
}

void position_loot( loot_t* loot ) {
    loot->x = GetRandomValue( -10, 10 );
    loot->y = GetRandomValue( -10, 10 );
}

int main(void)
{
    // Initialization
    //--------------------------------------------------------------------------------------
    const int screenWidth = 800;
    const int screenHeight = 450;

    InitWindow(screenWidth, screenHeight, "pingpong raylib");

    Rectangle playerRect = { 400, 280, 40, 40 };
    player_t player = { 0, 0 };
    loot_t loot;
    position_loot( &loot );

    Camera2D camera = { 0 };
    camera.rotation = 0.0f;
    camera.zoom = 1.0f;
    camera.offset = (Vector2){
        (screenWidth - playerRect.width) / 2,
        (screenHeight - playerRect.height) / 2 };

    SetTargetFPS(60);

    // Main game loop
    while (!WindowShouldClose())        // Detect window close button or ESC key
    {
        // Input / Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KEY_RIGHT) || IsKeyPressed( KEY_D ))
        {
            player.x++;
        }
        else if (IsKeyPressed(KEY_LEFT) || IsKeyPressed( KEY_A ))
        {
            player.x--;
        }
        if (IsKeyPressed(KEY_UP) || IsKeyPressed( KEY_W ))
        {
            player.y--;
        }
        else if (IsKeyPressed(KEY_DOWN)|| IsKeyPressed( KEY_S ))
        {
            player.y++;
        }
        
        // update the position
        playerRect.x = player.x * playerRect.width;
        playerRect.y = player.y * playerRect.height;
        camera.target = (Vector2){
            lerp(camera.target.x, playerRect.x, 0.1),
            lerp(camera.target.y, playerRect.y, 0.1) };

        //Did the player get the loot?
        while( loot.x == player.x && loot.y == player.y ){
            position_loot( &loot );
        }


        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            ClearBackground(RAYWHITE);

            BeginMode2D(camera);
            
                // draw a grid
                for (int x = -10; x <= 10; x++)
                {
                    for (int y = -10; y <= 10; y++)
                    {
                        int xw = x * playerRect.width;
                        int yh = y * playerRect.height;
                        DrawLine(xw, -1000, xw, 1000, GREEN);
                        DrawLine(-1000, yh, 1000, yh, GREEN);
                    }
                }
                
                // draw the player
                DrawRectangleRec(playerRect, RED);
                // draw the loot
                DrawRectangle( loot.x * playerRect.width, loot.y * playerRect.height, playerRect.width, playerRect.height, GOLD );

            EndMode2D();

            // show some UI text
            DrawRectangle( 10, 10, 250, 80, Fade(SKYBLUE, 0.5f));
            DrawRectangleLines( 10, 10, 250, 80, BLUE);
            DrawText("First test for a grid based game", 20, 20, 10, BLACK);
            DrawText("- Move with cursor keys", 40, 40, 10, DARKGRAY);
            DrawText("- Find the gold", 40, 60, 10, DARKGRAY);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    // De-Initialization
    //--------------------------------------------------------------------------------------
    CloseWindow();        // Close window and OpenGL context
    //--------------------------------------------------------------------------------------

    return 0;
}
