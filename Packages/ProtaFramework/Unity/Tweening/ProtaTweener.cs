using UnityEngine;
using Prota.Unity;
using Prota;
using System;
using TMPro;
using System.Collections.Generic;

namespace Prota.Unity
{
    public enum TweenTimerType : byte
    {
        Game = 0,
        // Physics = 1,
        Realtime = 2,
    }
    
    public class ProtaTweener : MonoBehaviour
    {
        public Transform cachedTransform;  // Cache transform to avoid repeated access
        public RectTransform cachedRectTransform => cachedTransform as RectTransform;  // Cache RectTransform if available

        public string animName = "";         // 用于唯一标记 Tweener.
        
        [Range(0, 1)] public float progress = 0;
        
        public bool play = false;        // 播放. 根据progress控制参数变化.
        
		public bool running = false;    // 自动播放. 根据时间流逝控制progress变化.
        
        // 时间流逝的类型. Game: 按时间流逝, Physics: 按物理时间流逝, Realtime: 按真实时间流逅.
        public TweenTimerType timerType = TweenTimerType.Game;
        
        // 播放完毕后是否停下来(设置 autoPlay = true).
        // 如果 loop = true, 则在起始位置(包括反向时的起始位置)停留. 否则, 在结束位置停留.
        public bool keepControlWhenFinish = false;
        
        public bool loop = false;
        
        // 指示当前的更新是从 0 到 1 还是反过来.
        public bool playReversed = false;
        
        // 循环一次后反转 from/to. false 即一直重复从0到1, true即从0到1再到0再到1循环.
        public bool reverseOnLoop = false;
        
        [SerializeField] ProtaTweenDefinitionPreset preset = null;
        [SerializeReference] TweenDefinition d = null;

		public TweenDefinition def => d != null ? d : preset.definition;

        public ProtaTweener[] children = Array.Empty<ProtaTweener>();
        
        // 播放时长. 决定了播放速度. 从 definition 获取.
        float duration => def.duration;
		
        void OnValidate()
        {
            if(preset != null) d = null;
            else if(d == null) d = new TweenDefinition();

			foreach (var child in children)
			{
				Debug.Assert(child != null, $"Child is null. {this.GetNamePath()}");
			}
        }
        
        void Awake()
        {
            cachedTransform = this.transform;

            if(preset != null && d != null)
            {
                Debug.LogWarning($"Tweener { this.GetNamePath() } has both preset and definition. Preset will be used.");
            }
			
            if(this.running && this.play)
                this.UpdateValue();
        }
		
		void OnEnable()
		{
			ProtaTweenerBatchUpdater.instance.AddTweener(this);
		}
		
		void OnDisable()
		{
			if(AppQuit.isQuitting) return;
			ProtaTweenerBatchUpdater.instance.RemoveTweener(this);
		}
        
        // void Update()
        // {
        //     switch(timerType)
        //     {
        //         case TweenTimerType.Game: Step(Time.deltaTime); break;
        //         case TweenTimerType.Realtime: Step(Time.unscaledDeltaTime); break;
        //         // case TweenTimerType.Physics: Step(Time.fixedDeltaTime); break;
        //     }
        // }
        
        // void Step(float dt)
        // {
		// 	var shouldUpdate = StepProgress(dt);
		// 	StepControl(shouldUpdate);
        // }
		
		public bool StepProgress(float dt)
		{
            if(!running) return false;
			
			// loop: 是否循环播放.
			// playReversed: 从1到0, progress减小; 否则, 从0到1, progress增大.
			// reverseOnLoop: 循环到达终点后, 反向播放.
			// keepControlWhenFinish: 播放完毕后是否停止.
			
            var original = progress;
			
			if(playReversed) progress -= dt / duration;
			else progress += dt / duration;
			
			if(loop)
			{
				if(reverseOnLoop)
				{
					var finished = (!playReversed && progress >= 1) || (playReversed && progress <= 0);
					if(finished && reverseOnLoop) playReversed = !playReversed;
					else if(finished && reverseOnLoop) playReversed = !playReversed;
					progress = progress.Clamp(0, 1);
				}
				else
				{
					progress = progress.Repeat(1);
				}
			}
			else
			{
				progress = progress.Clamp(0, 1);
				bool finished = playReversed ? progress == 0 : progress == 1;
				if(!keepControlWhenFinish && finished)
				{
					play = false;
					running = false;
					return true;
				}
			}
			
			return play && original != progress;
		}
		
		public void StepControl(bool shouldUpdate)
		{
            if(shouldUpdate) UpdateValue();
		}
        
        // ====================================================================================================
        // ====================================================================================================
        
        private Vector2 tweenedPosition
        {
            get => cachedRectTransform != null ? cachedRectTransform.anchoredPosition : cachedTransform.localPosition;
            set
            {
                if (cachedRectTransform != null)
                    cachedRectTransform.anchoredPosition = value;
                else
                    cachedTransform.localPosition = value;
            }
        }

        public void UpdateValue()
        {
			// move
			var d = this.def;
            if(d.move) tweenedPosition = d.GetMove(progress);
            // rotate
            if(d.rotate) cachedTransform.localEulerAngles = d.GetRotate(progress);
            // scale
            if(d.scale) cachedTransform.localScale = d.GetScale(progress);
            // color
            if(d.color || d.alpha)
            {
                var color = InternalGetColor();
				InternalSetColor(d.GetColorAlpha(ref color, progress));
            }
            // size
            if(d.size || d.sizeX || d.sizeY)
            {
                var size = InternalGetSize();
                InternalSetSize(d.GetSizeXY(ref size, progress));
            }
            
            // 子节点的运行状态由父节点控制.
			foreach(var child in children)
			{
				child.running = false;          // 子节点不会自己动.
				child.progress = progress;
				child.UpdateValue();
			}
        }
        
        public void Play() => play = running = true;
        
        public void Play(float startProgress)
        {
            Play();
            SetTo(startProgress);
        }
        
        public void PlayForwardRestart(float startProgress = 0)
        {
            playReversed = false;
            Play();
            SetTo(startProgress);
        }
        
        public void PlayBackwardRestart(float startProgress = 1)
        {
            playReversed = true;
            Play();
            SetTo(startProgress);
        }
		
		public void PlayForward()
		{
			playReversed = false;
			Play();
		}
		
		public void PlayBackward()
		{
			playReversed = true;
			Play();
		}
        
        // progress 在 0 到 1 之间.
        public void SetTo(float progress)
        {
            this.progress = progress;
            UpdateValue();
            ClearTweensInTheSameGameObject();
        }
        
        public void SetToFinish(bool keepRunning = false)
        {
			progress = 1;
			if(!keepRunning) running = false;
            UpdateValue();
        }
        
        public void SetToStart(bool keepRunning = false)
        {
			progress = 0;
			if(!keepRunning) running = false;
			UpdateValue();
        }
        
        public void SetTime(float time, bool keepRunning = false)
        {
			progress = time / duration;
			if(!keepRunning) running = false;
            UpdateValue();
        }
        
        public void ClearTweensInTheSameGameObject()
        {
            // 停止所有同级节点的运行. 它自己除外.
            var g = this.gameObject;
			var compCount = g.GetComponentCount();
            for(int i = 0; i < compCount; i++)
            {
                var comp = g.GetComponentAtIndex(i);
                if(ReferenceEquals(comp, this)) continue;
				if(comp is not ProtaTweener tweener) continue;
				if(tweener.animName != this.animName) continue;
				tweener.running = false;
				tweener.play = false;
				// Debug.Log($"Stop tweener. { this.GetNamePath() }");
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [NonSerialized] Component colorTarget;
        Func<Component, Color> colorGetter;
        Action<Component, Color> colorSetter;
        
        void AttachColorTarget()
        {
            this.gameObject.GetColorComponent(out colorTarget, out colorGetter, out colorSetter);
        }
        
        Color InternalGetColor()
        {
            if(colorTarget == null)
            {
                AttachColorTarget();
                if(colorTarget == null)
                {
                    Debug.LogError($"Color target is null. { this.GetNamePath() }", this);
                    return Color.white;
                }
            }
            
            return colorGetter(colorTarget);
        }
        
        void InternalSetColor(Color c) 
        {
            if(colorTarget == null)
            {
                AttachColorTarget();
                if(colorTarget == null)
                {
                    Debug.LogError($"Color target is null. { this.GetNamePath() }", this);
                    return;
                }
            }
            
            colorSetter(colorTarget, c);
        }
        
        [NonSerialized] Component sizeTarget;
        Func<Component, Vector2> sizeGetter;
        Action<Component, Vector2> sizeSetter;
        
        void AttachSizeTarget()
        {
            this.gameObject.GetSizeComponent(out sizeTarget, out sizeGetter, out sizeSetter);
        }
        
        Vector2 InternalGetSize()
        {
            if(sizeTarget == null)
            {
                AttachSizeTarget();
                if(sizeTarget == null)
                {
                    Debug.LogError($"Size target is null. { this.GetNamePath() }", this);
                    return Vector2.one;
                }
            }
            
            return sizeGetter(sizeTarget);
        }
        
        void InternalSetSize(Vector2 s) 
        {
            if(sizeTarget == null)
            {
                AttachSizeTarget();
                if(sizeTarget == null)
                {
                    Debug.LogError($"Size target is null. { this.GetNamePath() }", this);
                    return;
                }
            }
            
            sizeSetter(sizeTarget, s);
        }
        
        
        // ====================================================================================================
        // ====================================================================================================

        public void RecordTo()
        {
			var d = this.def;
            if (d.move) d.moveTo = tweenedPosition;
            if (d.rotate) d.rotateTo = cachedTransform.localEulerAngles;
            if (d.scale) d.scaleTo = cachedTransform.localScale;
            if(d.alpha) d.alphaTo = InternalGetColor().a;
            if (d.color) d.colorTo = InternalGetColor().WithA(d.alphaTo);
            if (d.size) d.sizeTo = InternalGetSize();
            if (d.sizeX) d.sizeXTo = InternalGetSize().x;
            if (d.sizeY) d.sizeYTo = InternalGetSize().y;
        }

		public void RecordFrom()
		{
			var d = this.def;
			if (d.move) d.moveFrom = tweenedPosition;
			if (d.rotate) d.rotateFrom = cachedTransform.localEulerAngles;
			if (d.scale) d.scaleFrom = cachedTransform.localScale;
			if (d.alpha) d.alphaFrom = InternalGetColor().a;
			if (d.color) d.colorFrom = InternalGetColor().WithA(d.alphaFrom);
			if (d.size) d.sizeFrom = InternalGetSize();
			if (d.sizeX) d.sizeXFrom = InternalGetSize().x;
			if (d.sizeY) d.sizeYFrom = InternalGetSize().y;
		}
    }
}

