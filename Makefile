TEST_RESULT = unity-test-result.xml
PACKAGE = com.vircadia.unitysdk.tgz
BUILDDIR = ./build

UNITY3D = $(UNITY3D_PATH)/Editor/Unity
DOXYGEN = doxygen

build:
	$(UNITY3D) -projectPath . -batchmode -nographics -quit -buildLinux64Player "$(BUILDDIR)/app" -logfile -

run:
	cd $(BUILDDIR); ./app -screen-fullscreen 0 -screen-height 600 -screen-width 800 -logfile -

debug:
	$(UNITY3D) -projectPath . -runTests -batchmode -nographics -testPlatform playmode -testResults $(TEST_RESULT) -logfile -

test:
	@echo Running tests...
	@$(UNITY3D) -projectPath . -runTests -batchmode -nographics -testPlatform playmode -testResults $(TEST_RESULT) -logfile - 2> /dev/null | grep VIRCADIA_SDK_TEST_LOG
	@xmlstarlet sel -t -m '//message[1]' -v . -n < $(TEST_RESULT) || true
	@xmlstarlet sel -t -m '//stack-trace[1]' -v . -n < $(TEST_RESULT) || true
	@echo Complete.

docs:
	-@rm -r ./docs/html ./docs/latex 2> /dev/null || true
	$(DOXYGEN) ./docs/Doxyfile
	-@rm -r Packages/com.vircadia.unitysdk/Documentation/html 2> /dev/null || true
	-@mkdir Packages/com.vircadia.unitysdk/Documentation 2> /dev/null || true
	cp ./docs/html Packages/com.vircadia.unitysdk/Documentation/ -r

package: docs test
	-rm ${PACKAGE}
	-rm -r Packages/com.vircadia.unitysdk/Samples~
	cp Assets/Samples Packages/com.vircadia.unitysdk/Samples~ -r
	tar --transform="s/Packages\/com.vircadia.unitysdk/package/" -czvf ${PACKAGE} Packages/com.vircadia.unitysdk

clean-tests:
	-rm $(TEST_RESULT)

clean-package:
	-rm $(PACKAGE)

clean-build:
	-rm -rf $(BUILDDIR)

.PHONY: build run debug test package docs clean-tests clean-package clean-build
