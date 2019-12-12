import setuptools

with open("README.md", "r") as fh:
    long_description = fh.read()

setuptools.setup(
    name="savanhdatabricks", 
    version="0.1.4",
    author="Sam Vanhoutte",
    author_email="sam.vanhoutte@marchitec.be",
    description="Databricks spark Azure packages",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/samvanhoutte/databricks",
    packages=setuptools.find_packages(),
    classifiers=[
        "Programming Language :: Python :: 3",
        "License :: OSI Approved :: MIT License",
        "Operating System :: OS Independent",
    ],
    install_requires = [
        'azure.eventgrid'
    ],
    python_requires='>=3.6',
    setup_requires=['wheel']
)