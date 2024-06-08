import matplotlib.pyplot as plt
import numpy as np
from collections import defaultdict
import os

class Event:
    def __init__(self, timestamp, event_type):
        self.timestamp = timestamp
        self.event_type = event_type
#每次绘制绘制一幅指定文件的图
experient_type = 0#0表示现在绘制仅选择，1表示仅操控，2表示仅

width = 60
height = 30
fig, ax = plt.subplots(figsize=(width, height))
colors = ['r', 'g', 'b', 'y']
def plot_timeline(events,event_typeToColor):
    # 定义颜色列表   
    # 排序事件,根据现有的总时间来排序
    events.sort(key=lambda x: x.timestamp)
    for i in events:
        print(i.timestamp)
    # 遍历每个事件类型
    finished_cnt = 0
    last_timestamp = 0
    for event in events:
        color = colors[event_typeToColor.index(event.event_type)]
        ax.add_patch(plt.Rectangle((last_timestamp, finished_cnt), event.timestamp-last_timestamp, 1, edgecolor='black', facecolor=color, alpha=0.6))
        # 添加文本标签
       # ax.text(last_timestamp, finished_cnt + 0.5, event.event_type, va='center', ha='right', fontsize=20, color='white' if color == 'r' else 'black')
        last_timestamp = event.timestamp
        if event.event_type.find("all") != -1:
            finished_cnt += 1
    
folders_selct = ["select_ours","select_bubble","select_vvir"]
event_typeToColor = []
events = []
def one_picture():
    with open("bubble.txt", 'r') as f:
        txt = f.read()
        # 解析时间戳数据
        for line in txt.split('\n'):
            print("____")
            print(line)
            line = line.replace(' ', '')
            if line.find("finishall") != -1:
                break
            if line.find(":") != -1  and line.find("this") == -1:
                event_type, timestamp = line.split(':')
                if event_type not in event_typeToColor:
                    event_typeToColor.append(event_type)
                timestamp = float(timestamp)
                events.append(Event(timestamp, event_type))
        print(event_typeToColor)
        plot_timeline(events, "picture", event_typeToColor)        


def multipul_picture():
    # 遍历当前文件夹下的所有 txt 文件
    for filename in os.listdir('select_ours'):
        events = []
        if filename.endswith('.txt'):
            with open('select_ours/'+filename, 'r') as f:
                txt = f.read()
                # 解析时间戳数据
                for line in txt.split('\n'):
                    line = line.replace(' ', '')
                    if line.find("finishall") != -1:
                        break
                    if line.find(":") != -1  and line.find("this") == -1 and line.find("precision"):
                        event_type, timestamp = line.split(':')
                        if event_type not in event_typeToColor:
                            event_typeToColor.append(event_type)
                        timestamp = float(timestamp)
                        print(timestamp)
                        events.append(Event(timestamp, event_type))
                print(event_typeToColor)
                plot_timeline(events, "picture", event_typeToColor)     

folders_selct = ["select_ours", "select_bubble", "select_vvir"]
event_typeToColor = []


def read_events_from_file(filename):
    all_events = []
    with open(filename, 'r') as f:
        txt = f.read()
        for line in txt.split('\n'):
            line = line.replace(' ', '')
            if line.find("finishall") != -1:
                    break
            if line.find(":") != -1 and line.find("this") == -1 and line.find("precision") == -1:
                event_type, timestamp = line.split(':')
                if event_type not in event_typeToColor:
                    event_typeToColor.append(event_type)
                print(event_typeToColor)
                timestamp = float(timestamp)
                all_events.append(Event(timestamp, event_type))
    return all_events

def plot_multiple_files():
    for filename in os.listdir('select_ours'):
        file_path = os.path.join('select_ours', filename)  # 假设每个文件夹下都有一个bubble.txt文件
        all_events = read_events_from_file(file_path)
        plot_timeline(all_events, event_typeToColor)

# 调用函数绘制多张图合并的图
plot_multiple_files()
# 设置坐标轴标签和标题
ax.set_xlim(0, width)
ax.set_ylim(0, height)
ax.tick_params(axis='both', which='both', labelsize=80)
ax.set_xlabel('Time (s)', fontsize=100)
ax.set_ylabel('Finished Number', fontsize=100)
ax.set_title('Operation Timeline', fontsize=100)
# 创建颜色条
legend_elements = [plt.Rectangle((0,0),1,1, color=c, label=et) for c, et in zip(colors, event_typeToColor)]
ax.legend(handles=legend_elements, loc='lower right', bbox_to_anchor=(1.1, 1.1),fontsize=50) 
# 调整图片布局
fig.tight_layout()
plt.savefig("combined_picture.png")
