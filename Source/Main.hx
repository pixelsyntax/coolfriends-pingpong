package;

import openfl.events.Event;
import openfl.utils.Assets;
import openfl.display.Bitmap;
import openfl.display.Sprite;

class Main extends Sprite
{
	var bitmap:Bitmap;
	var dir = 1;

	//

	public function new()
	{
		super();

		// please remove all this, i was just testing stuff
		
		var bitmapData = Assets.getBitmapData("assets/openfl.png");
		bitmap = new Bitmap(bitmapData);
		addChild(bitmap);

		addEventListener(Event.ENTER_FRAME, test);
	}

	function test(event:Event) {
		bitmap.x += dir * 5;
		if (bitmap.x > 200) { dir = -1; }
		else if (bitmap.x < 0) { dir = 1; }
	}
}