#include <mach/mach.h>
#import "WXApi.h"

extern "C"
{
    void WechatPay(const char* appid,const char* partnerid,const char* prepayid,const char* package,const char* noncestr,const char* timestamp,const char* sign)
    {
        //需要创建这个支付对象
        PayReq *req   = [[PayReq alloc] init];
        // 商家id，在注册的时候给的
        req.partnerId = [[NSString alloc] initWithUTF8String:partnerid];
        // 预支付订单这个是后台跟微信服务器交互后，微信服务器传给你们服务器的，你们服务器再传给你
        req.prepayId  = [[NSString alloc] initWithUTF8String:prepayid];
        // 根据财付通文档填写的数据和签名
        req.package  = [[NSString alloc] initWithUTF8String:package];
        // 随机编码，为了防止重复的，在后台生成
        req.nonceStr  = [[NSString alloc] initWithUTF8String:noncestr];
        // 这个是时间戳，也是在后台生成的，为了验证支付的
        NSString * stamp = [[NSString alloc] initWithUTF8String:timestamp];
        req.timeStamp = stamp.intValue;
        // 这个签名也是后台做的
        req.sign = [[NSString alloc] initWithUTF8String:sign];
        //发送请求到微信，等待微信返回onResp
        [WXApi sendReq:req];
    }
    
    void WechatPayWeChatInitialize(const char* appid)
    {
    }
}
