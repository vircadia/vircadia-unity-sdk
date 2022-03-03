TEST_RESULT = unity-test-result.xml
PACKAGE = com.vircadia.unitysdk.tar.gz

UNITY3D = $(UNITY3D_PATH)/Editor/Unity

debug:
	$(UNITY3D) -runTests -batchmode -nographics -testPlatform playmode -testResults $(TEST_RESULT) -logfile -

test:
	@echo Running tests...
	-@$(UNITY3D) -runTests -batchmode -nographics -testPlatform playmode -testResults $(TEST_RESULT) -logfile - 2> /dev/null | grep VIRCADIA_SDK_TEST_LOG
	@xmlstarlet sel -t -m '//message[1]' -v . -n < $(TEST_RESULT) || true
	@xmlstarlet sel -t -m '//stack-trace[1]' -v . -n < $(TEST_RESULT) || true
	@echo Complete.

package: test
	tar --xform s:'./':: -czvf $(PACKAGE) -C ./Packages/com.vircadia.unitysdk ./

docs:
	doxygen ./docs/Doxyfile

clean-tests:
	-rm $(TEST_RESULT)

clean-package:
	-rm $(PACKAGE)


.PHONY: test clean-tests
