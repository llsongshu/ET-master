﻿using System;
using System.Threading;

namespace ET
{
    [FriendOf(typeof(NetThreadComponent))]
    public static class NetThreadComponentSystem
    {
        [ObjectSystem]
        public class NetThreadComponentAwakeSystem: AwakeSystem<NetThreadComponent>
        {
            protected override void Awake(NetThreadComponent self)
            {
                NetThreadComponent.Instance = self;
            
                self.ThreadSynchronizationContext = ThreadSynchronizationContext.Instance;
            }
        }

        [ObjectSystem]
        public class NetThreadComponentUpdateSystem: LateUpdateSystem<NetThreadComponent>
        {
            protected override void LateUpdate(NetThreadComponent self)
            {
                foreach (AService service in self.Services)
                {
                    service.Update();
                }
            }
        }
    
        [ObjectSystem]
        public class NetThreadComponentDestroySystem: DestroySystem<NetThreadComponent>
        {
            protected override void Destroy(NetThreadComponent self)
            {
                self.Stop();
            }
        }

        public static void Stop(this NetThreadComponent self)
        {
        }

        public static void Add(this NetThreadComponent self, AService kService)
        {
            // 这里要去下一帧添加，避免foreach错误
            self.ThreadSynchronizationContext.PostNext(() =>
            {
                if (kService.IsDispose())
                {
                    return;
                }
                self.Services.Add(kService);
            });
        }
        
        public static void Remove(this NetThreadComponent self, AService kService)
        {
            // 这里要去下一帧删除，避免foreach错误
            self.ThreadSynchronizationContext.PostNext(() =>
            {
                if (kService.IsDispose())
                {
                    return;
                }
                self.Services.Remove(kService);
            });
        }
        
    }
}