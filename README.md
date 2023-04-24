## 描述
### 本项目是 @github/ljc545w 大佬的[ComWechatRobot](https://github.com/ljc545w/ComWeChatRobot) C#版实现
### 本项目不再维护，之后将使用ljc545w大佬的上述项目的http模式，见[RS.JJJ.Wechat](https://github.com/RingoStudio/RS.JJJ.Wechat)

### 目前实现以下功能：
- 获取通讯录
- 获取所有群成员基础信息(含wxid，群昵称，微信昵称）
- 发送文本、图片、文件、群艾特（含多个艾特）消息
- 自动合并短的文本、艾特信息（可设定单条信息最大长度）
- 自动分割过长的单条信息


## 可用版本
微信3.7.0.26 [下载地址](https://aichunjing.lanzoui.com/b00dd197e)


## 编译环境
Visual Studio 2022 + .Net 5.0


## 使用前准备
1. 下载[ComWechatBot的必要文件](https://github.com/RingoStudio/ComWechatRobotCsharp/edit/master/com)，放在exe同一目录下！
2. 使用管理员命令行注册或卸载com组件
```
# 安装
CWeChatRobot.exe /regserver
# 卸载
CWeChatRobot.exe /unregserver
```
3. 准备注入器[WechatInjector](https://github.com/RingoStudio/WechatInjector)

## 更多功能
后续计划功能：
- 跟进 [ComWechatRobot](https://github.com/ljc545w/ComWeChatRobot) 更新
- 后续提供语音、图片接收等功能

最近几天在给ljc545w大佬添麻烦，想要帮助改进项目
期待大佬的好消息！


## 更新记录
#### 2022.06.13
- 首次发布
#### 2022.06.14
- 跟进ComWechatRobot更改At消息参数，在本项目中，为了最终呈现的消息文本的整洁，所有的@昵称在传入时添加，而不是由Robot自动在最前面添加。

## 免责声明
代码仅供交流学习使用，请勿用于非法用途和商业用途！如因此产生任何法律纠纷，均与作者无关！
