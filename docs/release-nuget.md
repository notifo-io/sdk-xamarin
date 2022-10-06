# Release NuGet package
This document describes how to publish a new version of the NuGet package for Notifo SDK for Xamarin.

We use [Github Actions](https://docs.github.com/en/actions) to release NuGet packages. Releases are based on [Git tags](https://git-scm.com/book/en/Git-Basics-Tagging), which mark a specific point in the repository's history.  Follow the steps below to publish a new version of packages to [nuget.org](https://nuget.org).

* Navigate to the main page of the repository.
* Under the repository name, click **Releases**.
* Click **Draft a new release**.

* Type a version number for your release. Versions are based on [Git tags](https://git-scm.com/book/en/Git-Basics-Tagging).  Examples: 
    * v1.0.0
    * v0.0.1-alpha1
    *(Note: suffix **-alpha**, **-beta** will publish [pre-release](https://docs.microsoft.com/en-us/nuget/concepts/package-versioning#pre-release-versions) version of NuGet package.)*
![Releases tagged version](https://docs.github.com/assets/cb-40521/images/help/releases/releases-tag-create-confirm.png)
* Type a title and description for your release.
![Releases description](https://docs.github.com/assets/cb-15127/images/help/releases/releases_description_auto.png)
* To notify users that the release is not ready for production and may be unstable, select **This is a pre-release**.
![Checkbox to mark a release as prerelease](https://help.github.com/assets/images/help/releases/prerelease_checkbox.png)
* Click **Publish release**. It will trigger the workflow in [publish.yml](https://github.com/notifo-io/sdk-xamarin/blob/master/.github/workflows/publish.yml) and after a few minutes, the new release should be available in [nuget.org](https://www.nuget.org/packages?q=Notifo).
![Publish release and Draft release buttons](https://help.github.com/assets/images/help/releases/release_buttons.png)



