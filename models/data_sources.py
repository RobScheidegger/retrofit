"""
This file contains the methods to load various data for the project.

Including images, videos, and their respective metadata.
"""

import os
import pandas as pd
from PIL import Image
import random

from dataclasses import dataclass

IMAGE_EXTENSIONS = ["jpg", "jpeg", "png"]
VIDEO_EXTENSIONS = ["mp4", "mov", "avi"]
ALL_EXTENSIONS = IMAGE_EXTENSIONS + VIDEO_EXTENSIONS


def load_items_from_path(base_path: str, fraction: float = 1) -> pd.DataFrame:
    """
    Recursively load all retrofit items from a directory and all of its subdirectories.

    The `fraction` parameter can be used to load only a fraction of the data (selected randomly).
    """
    files_read = 0
    total_file_size = 0
    metadata_list = []

    for root, dirs, files in os.walk(base_path):
        for file in files:
            if random.random() % 1 > fraction:
                continue

            extension = file.split(".")[-1].lower()

            file_size = os.path.getsize(os.path.join(root, file))

            files_read += 1
            total_file_size += file_size

            print(
                "\r Importing data from {} ({} files read, {} GB found)".format(
                    base_path, files_read, total_file_size / 1e9
                ),
                end="",
            )

            width = None
            height = None

            if extension in ["jpg", "jpeg", "png"]:
                try:
                    img = Image.open(os.path.join(root, file), formats=["PNG", "JPEG"])

                    width = img.width
                    height = img.height
                except:
                    print("\n Could not read image {}".format(file))

            metadata = {
                "name": file,
                "file_type": file.split(".")[-1],
                "date_created": os.path.getctime(os.path.join(root, file)),
                "extension": extension,
                "file_size": file_size,
                "width": width,
                "height": height,
            }

            metadata_list.append(metadata)

    print("\n")

    data = pd.DataFrame(metadata_list)

    return data
