import matplotlib.pyplot as plt
import numpy as np
from collections import defaultdict
import os

fig, ax = plt.subplots(figsize=(120, 40))

def plot_timeline(data_dict, png_filename, event_typeToColor):
    # 定义颜色列表
    colors = ['r', 'g', 'b', 'y']

    # 遍历每个事件类型
    finished_cnt = 0
    last_timestamp = 0
    for (timestamps, event_type) in enumerate(data_dict.items()):
        color = colors[event_typeToColor.index(event_type[1])]
        ax.add_patch(plt.Rectangle((last_timestamp, finished_cnt), event_type[0]-last_timestamp, 1, edgecolor = 'black', facecolor=color, alpha=0.6))
        last_timestamp = event_type[0]
        if(event_type[1].find("all") != -1):
            finished_cnt += 1

    # 设置坐标轴标签和标题
    ax.set_xlim(0, 40)
    ax.set_ylim(0, 40)
    ax.tick_params(axis='both', which='both', labelsize=80)
    ax.set_xlabel('Time (s)', fontsize=100)
    ax.set_ylabel('Finished Number', fontsize=100)
    ax.set_title('Operation Timeline', fontsize=100)

    # 调整图片布局
    fig.tight_layout()

# 遍历当前文件夹下的所有 txt 文件
for filename in os.listdir('.'):
    if filename.endswith('.txt'):
        data_dict = defaultdict(list)
        event_typeToColor = ["allSelectionTime", "onPalmPoseStart", "onPalmPoseExit", "onSecondSelectionBGDisappear"]
        # 尝试使用不同的编码方式读取文件
        for encoding in ['utf-8', 'gbk', 'latin-1']:
            try:
                with open(filename, 'r', encoding=encoding) as f:
                    txt = f.read()
                break
            except UnicodeDecodeError:
                continue
        else:
            print(f"Failed to decode {filename}")
            continue

        # 解析时间戳数据
        for line in txt.split('\n'):
            line = line.replace(' ', '')
            if line.find(":") != -1 and line.find("selectObject") == -1 and line.find("this") == -1:
                event_type, timestamp = line.split(':')
                timestamp = float(timestamp)
                data_dict[timestamp] = event_type

        plot_timeline(data_dict, filename, event_typeToColor)
        
plt.savefig(f"sum.png")