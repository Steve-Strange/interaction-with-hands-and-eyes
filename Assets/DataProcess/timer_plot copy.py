import matplotlib.pyplot as plt
import numpy as np
import os

class Event:
    def __init__(self, timestamp, event_type):
        self.timestamp = timestamp
        self.event_type = event_type

# 定义全局变量
width = 60
height = 30

def plot_timeline(events, ax, event_typeToColor, last_timestamp):
    colors = ['r', 'g', 'b', 'y']  # 定义颜色列表

    # 遍历事件并绘制
    for event in events:
        color = colors[event_typeToColor.index(event.event_type)]
        ax.add_patch(plt.Rectangle((last_timestamp, 0), event.timestamp - last_timestamp, 1, 
                                   edgecolor='black', facecolor=color, alpha=0.6))
        last_timestamp = event.timestamp

def read_events_from_file(filename):
    events = []
    with open(filename, 'r') as f:
        for line in f:
            line = line.replace(' ', '')
                    
            if line.find("finishall") != -1:
                        break
            if ":" in line and "this" not in line and line.find("precision")==-1:
                event_type, timestamp = line.strip().split(':')
                events.append(Event(float(timestamp), event_type))
    return events

def plot_multiple_files(folders_selct, output_filename):
    event_typeToColor = ['onPalmPoseStart', 'onPalmPoseExit', 'allSelectionTime', 'onSecondSelectionBGDisappear']  # 使用defaultdict来自动分配颜色
    last_timestamp = 0  # 用于记录上一个事件的时间戳

    fig, ax = plt.subplots(figsize=(width, height))
    for filename in os.listdir('select_ours'):
        file_path = os.path.join('select_ours', filename) 
        events = read_events_from_file(file_path)
        plot_timeline(events, ax, event_typeToColor, last_timestamp)
        last_timestamp = max(event.timestamp for event in events)  # 更新为当前文件中的最大时间戳

    # 设置坐标轴标签和标题
    ax.set_xlim(0, last_timestamp)  # 使用最大时间戳作为x轴的上限
    ax.set_ylim(-0.5, len(event_typeToColor) - 0.5)  # y轴范围为事件类型的数量
    ax.tick_params(axis='both', which='both', labelsize=10)
    ax.set_xlabel('Time (s)', fontsize=12)
    ax.set_ylabel('Event Type', fontsize=12)
    ax.set_title('Operation Timeline', fontsize=14)
    colors = ['r', 'g', 'b', 'y']  # 定义颜色列表
    # 创建颜色条
    legend_elements = [plt.Rectangle((0,0),1,1, color=c, label=et) for c, et in zip(colors, event_typeToColor)]
    ax.legend(handles=legend_elements, loc='upper right', bbox_to_anchor=(1.1, 1.1), fontsize=10)

    # 调整图片布局并保存
    fig.tight_layout()
    plt.savefig(output_filename)

# 示例用法
folders_selct = ["select_ours", "select_bubble", "select_vvir"]
output_filename = "combined_picture.png"
plot_multiple_files(folders_selct, output_filename)