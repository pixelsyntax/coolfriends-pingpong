package;

import openfl.events.Event;
import openfl.utils.Assets;
import openfl.display.Bitmap;
import openfl.display.Sprite;
import haxe.Timer;
class Main extends Sprite
{
	var bitmap : Bitmap;
	var ringBitmaps:Array<Bitmap>;
	var dir = 1;

	//

	public function new()
	{
		super();

		// please remove all this, i was just testing stuff
		bitmap = new Bitmap( Assets.getBitmapData("assets/openfl.png") );
		addChild( bitmap );

		ringBitmaps = new Array<Bitmap>();
		var ringBitmapData = Assets.getBitmapData("assets/ring.png");
		for ( i in 0...25 ){
			var ringBitmap = new Bitmap(ringBitmapData);
			ringBitmaps.push( ringBitmap );
			addChild(ringBitmap);
		}
		
		addEventListener(Event.ENTER_FRAME, test);
	}

	function test(event:Event) {
		var t = Timer.stamp();
		var i = 0;
		var tau = Math.PI * 2;
		var tauByLength = tau / ringBitmaps.length;
		var radiusX = 100 + Math.sin( t ) * 50;
		var radiusY = 100 + Math.sin( t * 1.2 ) * 50;
		for ( ringBitmap in ringBitmaps ){
			ringBitmap.x = Math.sin( t/100 + tauByLength * i ) * radiusX + stage.stageWidth/2;
			ringBitmap.y = Math.cos( t*3 + tauByLength * i ) * radiusY + stage.stageHeight/2;
			++i;
		}
		
		bitmap.x += dir * 5;
		if ( bitmap.x + bitmap.width > stage.stageWidth )
			dir = -1;
		if ( bitmap.x < 0 )
			dir = 1;
	}
}