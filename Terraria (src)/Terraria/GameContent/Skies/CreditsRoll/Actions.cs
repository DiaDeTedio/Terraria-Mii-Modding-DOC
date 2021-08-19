using Microsoft.Xna.Framework;

namespace Terraria.GameContent.Skies.CreditsRoll
{
	public class Actions
	{
		public class Players
		{
			public interface IPlayerAction : ICreditsRollSegmentAction<Player>
			{
			}

			public class Fade : IPlayerAction, ICreditsRollSegmentAction<Player>
			{
				private int _duration;

				private float _opacityTarget;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Fade(float opacityTarget)
				{
					_duration = 0;
					_opacityTarget = opacityTarget;
				}

				public Fade(float opacityTarget, int duration)
				{
					_duration = duration;
					_opacityTarget = opacityTarget;
				}

				public void BindTo(Player obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Player obj, float localTimeForObj)
				{
					if (localTimeForObj < _delay)
					{
						return;
					}
					if (_duration == 0)
					{
						obj.opacityForCreditsRoll = _opacityTarget;
						return;
					}
					float num = localTimeForObj - _delay;
					if (num > (float)_duration)
					{
						num = _duration;
					}
					obj.opacityForCreditsRoll = MathHelper.Lerp(obj.opacityForCreditsRoll, _opacityTarget, Utils.GetLerpValue(0f, _duration, num, clamped: true));
				}
			}

			public class Wait : IPlayerAction, ICreditsRollSegmentAction<Player>
			{
				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Wait(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(Player obj)
				{
				}

				public void ApplyTo(Player obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						obj.velocity = Vector2.Zero;
					}
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class LookAt : IPlayerAction, ICreditsRollSegmentAction<Player>
			{
				private int _direction;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => 0;

				public LookAt(int direction)
				{
					_direction = direction;
				}

				public void BindTo(Player obj)
				{
				}

				public void ApplyTo(Player obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						obj.direction = _direction;
					}
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class MoveWithAcceleration : IPlayerAction, ICreditsRollSegmentAction<Player>
			{
				private Vector2 _offsetPerFrame;

				private Vector2 _accelerationPerFrame;

				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public MoveWithAcceleration(Vector2 offsetPerFrame, Vector2 accelerationPerFrame, int durationInFrames)
				{
					_accelerationPerFrame = accelerationPerFrame;
					_offsetPerFrame = offsetPerFrame;
					_duration = durationInFrames;
				}

				public void BindTo(Player obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Player obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						Vector2 vector = _offsetPerFrame * num + _accelerationPerFrame * (num * num * 0.5f);
						obj.position += vector;
						obj.velocity = _offsetPerFrame + _accelerationPerFrame * num;
						if (_offsetPerFrame.X != 0f)
						{
							obj.direction = ((_offsetPerFrame.X > 0f) ? 1 : (-1));
						}
					}
				}
			}
		}

		public class NPCs
		{
			public interface INPCAction : ICreditsRollSegmentAction<NPC>
			{
			}

			public class Fade : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _duration;

				private int _alphaPerFrame;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Fade(int alphaPerFrame)
				{
					_duration = 0;
					_alphaPerFrame = alphaPerFrame;
				}

				public Fade(int alphaPerFrame, int duration)
				{
					_duration = duration;
					_alphaPerFrame = alphaPerFrame;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (localTimeForObj < _delay)
					{
						return;
					}
					if (_duration == 0)
					{
						obj.alpha = Utils.Clamp(obj.alpha + _alphaPerFrame, 0, 255);
						return;
					}
					float num = localTimeForObj - _delay;
					if (num > (float)_duration)
					{
						num = _duration;
					}
					obj.alpha = Utils.Clamp(obj.alpha + (int)num * _alphaPerFrame, 0, 255);
				}
			}

			public class ShowItem : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _itemIdToShow;

				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public ShowItem(int durationInFrames, int itemIdToShow)
				{
					_duration = durationInFrames;
					_itemIdToShow = itemIdToShow;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							FixNPCIfWasHoldingItem(obj);
							return;
						}
						obj.velocity = Vector2.Zero;
						obj.frameCounter = num;
						obj.ai[0] = 23f;
						obj.ai[1] = (float)_duration - num;
						obj.ai[2] = _itemIdToShow;
					}
				}

				private void FixNPCIfWasHoldingItem(NPC obj)
				{
					if (obj.ai[0] == 23f)
					{
						obj.frameCounter = 0.0;
						obj.ai[0] = 0f;
						obj.ai[1] = 0f;
						obj.ai[2] = 0f;
					}
				}
			}

			public class Move : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private Vector2 _offsetPerFrame;

				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Move(Vector2 offsetPerFrame, int durationInFrames)
				{
					_offsetPerFrame = offsetPerFrame;
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						obj.position += _offsetPerFrame * num;
						obj.velocity = _offsetPerFrame;
						if (_offsetPerFrame.X != 0f)
						{
							obj.direction = (obj.spriteDirection = ((_offsetPerFrame.X > 0f) ? 1 : (-1)));
						}
					}
				}
			}

			public class MoveWithAcceleration : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private Vector2 _offsetPerFrame;

				private Vector2 _accelerationPerFrame;

				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public MoveWithAcceleration(Vector2 offsetPerFrame, Vector2 accelerationPerFrame, int durationInFrames)
				{
					_accelerationPerFrame = accelerationPerFrame;
					_offsetPerFrame = offsetPerFrame;
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						Vector2 vector = _offsetPerFrame * num + _accelerationPerFrame * (num * num * 0.5f);
						obj.position += vector;
						obj.velocity = _offsetPerFrame + _accelerationPerFrame * num;
						if (_offsetPerFrame.X != 0f)
						{
							obj.direction = (obj.spriteDirection = ((_offsetPerFrame.X > 0f) ? 1 : (-1)));
						}
					}
				}
			}

			public class MoveWithRotor : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private Vector2 _offsetPerFrame;

				private Vector2 _resultMultiplier;

				private float _radialOffset;

				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public MoveWithRotor(Vector2 radialOffset, float rotationPerFrame, Vector2 resultMultiplier, int durationInFrames)
				{
					_radialOffset = rotationPerFrame;
					_offsetPerFrame = radialOffset;
					_resultMultiplier = resultMultiplier;
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						Vector2 vector = _offsetPerFrame.RotatedBy(_radialOffset * num) * _resultMultiplier;
						obj.position += vector;
					}
				}
			}

			public class DoBunnyRestAnimation : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public DoBunnyRestAnimation(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (localTimeForObj < _delay)
					{
						return;
					}
					float num = localTimeForObj - _delay;
					if (num > (float)_duration)
					{
						num = _duration;
					}
					Rectangle frame = obj.frame;
					int num2 = 10;
					int num3 = (int)num;
					while (num3 > 4)
					{
						num3 -= 4;
						num2++;
						if (num2 > 13)
						{
							num2 = 13;
						}
					}
					obj.ai[0] = 21f;
					obj.ai[1] = 31f;
					obj.frameCounter = num3;
					obj.frame.Y = num2 * frame.Height;
				}
			}

			public class Wait : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Wait(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						obj.velocity = Vector2.Zero;
					}
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class Blink : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Blink(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						obj.velocity = Vector2.Zero;
						obj.ai[0] = 0f;
						if (!(localTimeForObj > _delay + (float)_duration))
						{
							obj.ai[0] = 1001f;
						}
					}
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class LookAt : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _direction;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => 0;

				public LookAt(int direction)
				{
					_direction = direction;
				}

				public void BindTo(NPC obj)
				{
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						obj.direction = (obj.spriteDirection = _direction);
					}
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class PartyHard : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				public int ExpectedLengthOfActionInFrames => 0;

				public void BindTo(NPC obj)
				{
					obj.ForcePartyHatOn = true;
					obj.UpdateAltTexture();
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
				}

				public void SetDelay(float delay)
				{
				}
			}

			public class ForceAltTexture : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _altTexture;

				public int ExpectedLengthOfActionInFrames => 0;

				public ForceAltTexture(int altTexture)
				{
					_altTexture = altTexture;
				}

				public void BindTo(NPC obj)
				{
					obj.altTexture = _altTexture;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
				}

				public void SetDelay(float delay)
				{
				}
			}

			public class Variant : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _variant;

				public int ExpectedLengthOfActionInFrames => 0;

				public Variant(int variant)
				{
					_variant = variant;
				}

				public void BindTo(NPC obj)
				{
					obj.townNpcVariationIndex = _variant;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
				}

				public void SetDelay(float delay)
				{
				}
			}

			public class ZombieKnockOnDoor : INPCAction, ICreditsRollSegmentAction<NPC>
			{
				private int _duration;

				private float _delay;

				private Vector2 bumpOffset = new Vector2(-1f, 0f);

				private Vector2 bumpVelocity = new Vector2(0.75f, 0f);

				public int ExpectedLengthOfActionInFrames => _duration;

				public ZombieKnockOnDoor(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(NPC obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(NPC obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						if ((int)num % 60 / 4 <= 3)
						{
							obj.position += bumpOffset;
							obj.velocity = bumpVelocity;
						}
						else
						{
							obj.position -= bumpOffset;
							obj.velocity = Vector2.Zero;
						}
					}
				}
			}
		}

		public class Sprites
		{
			public interface ISpriteAction : ICreditsRollSegmentAction<Segments.LooseSprite>
			{
			}

			public class Fade : ISpriteAction, ICreditsRollSegmentAction<Segments.LooseSprite>
			{
				private int _duration;

				private float _opacityTarget;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Fade(float opacityTarget)
				{
					_duration = 0;
					_opacityTarget = opacityTarget;
				}

				public Fade(float opacityTarget, int duration)
				{
					_duration = duration;
					_opacityTarget = opacityTarget;
				}

				public void BindTo(Segments.LooseSprite obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Segments.LooseSprite obj, float localTimeForObj)
				{
					if (localTimeForObj < _delay)
					{
						return;
					}
					if (_duration == 0)
					{
						obj.CurrentOpacity = _opacityTarget;
						return;
					}
					float num = localTimeForObj - _delay;
					if (num > (float)_duration)
					{
						num = _duration;
					}
					obj.CurrentOpacity = MathHelper.Lerp(obj.CurrentOpacity, _opacityTarget, Utils.GetLerpValue(0f, _duration, num, clamped: true));
				}
			}

			public class Wait : ISpriteAction, ICreditsRollSegmentAction<Segments.LooseSprite>
			{
				private int _duration;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => _duration;

				public Wait(int durationInFrames)
				{
					_duration = durationInFrames;
				}

				public void BindTo(Segments.LooseSprite obj)
				{
				}

				public void ApplyTo(Segments.LooseSprite obj, float localTimeForObj)
				{
					_ = _delay;
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}
			}

			public class SimulateGravity : ISpriteAction, ICreditsRollSegmentAction<Segments.LooseSprite>
			{
				private int _duration;

				private float _delay;

				private Vector2 _initialVelocity;

				private Vector2 _gravityPerFrame;

				private float _rotationPerFrame;

				public int ExpectedLengthOfActionInFrames => _duration;

				public SimulateGravity(Vector2 initialVelocity, Vector2 gravityPerFrame, float rotationPerFrame, int duration)
				{
					_duration = duration;
					_initialVelocity = initialVelocity;
					_gravityPerFrame = gravityPerFrame;
					_rotationPerFrame = rotationPerFrame;
				}

				public void BindTo(Segments.LooseSprite obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Segments.LooseSprite obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						float num = localTimeForObj - _delay;
						if (num > (float)_duration)
						{
							num = _duration;
						}
						Vector2 vector = _initialVelocity * num + _gravityPerFrame * (num * num);
						obj.CurrentDrawData.position += vector;
						obj.CurrentDrawData.rotation += _rotationPerFrame * num;
					}
				}
			}

			public class SetFrame : ISpriteAction, ICreditsRollSegmentAction<Segments.LooseSprite>
			{
				private int _targetFrameX;

				private int _targetFrameY;

				private int _paddingX;

				private int _paddingY;

				private float _delay;

				public int ExpectedLengthOfActionInFrames => 0;

				public SetFrame(int frameX, int frameY, int paddingX = 2, int paddingY = 2)
				{
					_targetFrameX = frameX;
					_targetFrameY = frameY;
					_paddingX = paddingX;
					_paddingY = paddingY;
				}

				public void BindTo(Segments.LooseSprite obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Segments.LooseSprite obj, float localTimeForObj)
				{
					if (!(localTimeForObj < _delay))
					{
						Rectangle value = obj.CurrentDrawData.sourceRect.Value;
						value.X = (value.Width + _paddingX) * _targetFrameX;
						value.Y = (value.Height + _paddingY) * _targetFrameY;
						obj.CurrentDrawData.sourceRect = value;
					}
				}
			}

			public class SetFrameSequence : ISpriteAction, ICreditsRollSegmentAction<Segments.LooseSprite>
			{
				private Point[] _frameIndices;

				private int _timePerFrame;

				private int _paddingX;

				private int _paddingY;

				private float _delay;

				private int _duration;

				private bool _loop;

				public int ExpectedLengthOfActionInFrames => _duration;

				public SetFrameSequence(int duration, Point[] frameIndices, int timePerFrame, int paddingX = 2, int paddingY = 2)
					: this(frameIndices, timePerFrame, paddingX, paddingY)
				{
					_duration = duration;
					_loop = true;
				}

				public SetFrameSequence(Point[] frameIndices, int timePerFrame, int paddingX = 2, int paddingY = 2)
				{
					_frameIndices = frameIndices;
					_timePerFrame = timePerFrame;
					_paddingX = paddingX;
					_paddingY = paddingY;
					_duration = _timePerFrame * _frameIndices.Length;
				}

				public void BindTo(Segments.LooseSprite obj)
				{
				}

				public void SetDelay(float delay)
				{
					_delay = delay;
				}

				public void ApplyTo(Segments.LooseSprite obj, float localTimeForObj)
				{
					if (localTimeForObj < _delay)
					{
						return;
					}
					Rectangle value = obj.CurrentDrawData.sourceRect.Value;
					int num = 0;
					if (_loop)
					{
						int num2 = _frameIndices.Length;
						num = (int)(localTimeForObj % (float)(_timePerFrame * num2)) / _timePerFrame;
						if (num >= num2)
						{
							num = num2 - 1;
						}
					}
					else
					{
						float num3 = localTimeForObj - _delay;
						if (num3 > (float)_duration)
						{
							num3 = _duration;
						}
						num = (int)(num3 / (float)_timePerFrame);
						if (num >= _frameIndices.Length)
						{
							num = _frameIndices.Length - 1;
						}
					}
					Point point = _frameIndices[num];
					value.X = (value.Width + _paddingX) * point.X;
					value.Y = (value.Height + _paddingY) * point.Y;
					obj.CurrentDrawData.sourceRect = value;
				}
			}
		}
	}
}
