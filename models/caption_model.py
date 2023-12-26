"""
This file contains the model for captioning the image, as well as generate embeddings for the image.
"""

import numpy as np
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration


class RetrofitCaptionModel:
    def __init__(self):
        self.processor = processor = BlipProcessor.from_pretrained(
            "Salesforce/blip-image-captioning-large"
        )
        self.model = BlipForConditionalGeneration.from_pretrained(
            "Salesforce/blip-image-captioning-large"
        ).to("cuda")

    def generate_image_caption(self, image_path: str, image_format: str = "jpg") -> str:
        raw_image = Image.open(image_path).convert("RGB")
        image = self.processor(raw_image, return_tensors="pt").to("cuda")

        caption = self.model.generate(**image)
        return caption

    def generate_image_embedding(self, image) -> np.ndarray:
        raise NotImplementedError("Not implemented yet!")
